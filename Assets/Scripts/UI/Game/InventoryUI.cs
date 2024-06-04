using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using Unity.Netcode;

// support struct for storing patient information with extra details
[System.Serializable]
public struct PatientInformation
{
    public string informationName;
    public string value;
    public int clue;
}

public class InventoryUI : NetworkBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private UIManager UIManager; // the current player's UI manager
    [SerializeField] private bool Focused; // whether the mouse is on the clipboard

    private SharedInventory SharedInventory; // component with actual inventory data
    [SerializeField] private SharedNotes SharedNotes; // component with actual notes

    [SerializeField] private GameObject ItemButtonPrefab; // prefab for adding a new item to the UI
    private List<Button> ItemButtons; // memory for items list

    [SerializeField] private List<PatientInformation> PatientInformationList; // bullet points for sticky note

    // UI components to fill
    [SerializeField] private GameObject ItemListBox;
    [SerializeField] private Image ItemImage;
    [SerializeField] private TMP_Text ItemName, ItemDesc;
    [SerializeField] private Button UseItemButton;
    [SerializeField] private ScrollRect ItemScrollWindow;

    // UI components needed to switch views
    [SerializeField] private GameObject InventoryPanel, NotesPanel;
    [SerializeField] private Button InventoryButton, NotesButton;
    [SerializeField] private GameObject AlreadyViewedLabel;
    [SerializeField] private TMP_Text AlreadyViewedText;

    // UI components for the notes panel
    [SerializeField] private GameObject NotesListPanel;

    // UI components for the patient information sticky note
    [SerializeField] private TMP_Text InformationNote;

    // Event system for tracking what was clicked
    [SerializeField] private EventSystem EventSystem;

    // Updating UI in real-time
    private int ActiveItemIndex = -1;

    // Inventory folder sprites
    [SerializeField] private Sprite FolderPage1, FolderPage2;
    [SerializeField] private Image FolderImage;
    [SerializeField] private GameObject FolderItemPanel, FolderInformationPanel;
    [SerializeField] private Button ShowItemPanelButton, ShowInformationPanelButton;
    
    private void Start()
    {
        SharedInventory = FindObjectOfType<SharedInventory>();
        ItemButtons = new List<Button>();
        UseItemButton.interactable = false; // no items can be used at start

        EventSystem = FindObjectOfType<EventSystem>();

        SharedNotes = FindObjectOfType<SharedNotes>();

        // load default missing information onto sticky note
        UpdateInformationClientRpc(0);
    }

    // If a click occurs, defer to the UIManager to see if panel can be closed
    private void Update()
    {
        if (!Focused && Input.GetMouseButtonDown(0) && !(EventSystem.currentSelectedGameObject != null && EventSystem.currentSelectedGameObject.name == ChatLogUI.ChatlogObjectName))
        {
            UIManager.CloseInventory();
        }

        if (ItemButtons.Count > 0 && ActiveItemIndex >= 0)
        {
            InventoryItem item = SharedInventory.GetItemDetails(ActiveItemIndex);
            if (!item.IsStillNecessary)
            {
                AlreadyViewedLabel.SetActive(true);
                AlreadyViewedText.color = Color.green;
                AlreadyViewedText.text = "Item is no longer needed!";
            }
            else if (item.HasBeenViewed)
            {
                AlreadyViewedLabel.SetActive(true);
                AlreadyViewedText.color = Color.grey;
                AlreadyViewedText.text = "Teammate already used!";
            }
            else
            {
                AlreadyViewedLabel.SetActive(false);
            }
        }
    }

    // Detects if the mouse is in a UI element
    public void OnPointerEnter(PointerEventData eventData)
    {
        Focused = true;
    }

    // Detects if the mouse leaves a UI element
    public void OnPointerExit(PointerEventData eventData)
    {
        Focused = false;
    }

    // Adding an new item to the inventory by adding a button for it to the list
    public void AddItem(UtilityObject item)
    {
        GameObject newBtnObj = Instantiate(ItemButtonPrefab, ItemListBox.transform);
        Button newBtn = newBtnObj.GetComponent<Button>();
        int currentIndex = ItemButtons.Count;
        newBtn.onClick.AddListener(() =>
        {
            ActiveItemIndex = currentIndex;
            ShowDetails(currentIndex); // when selected, fill menu with details
        });
        TMP_Text label = newBtnObj.GetComponentInChildren<TMP_Text>();
        label.text = item.ItemDetails.Name;
        ItemButtons.Add(newBtn);

        // lower the item list to show the last item
        ItemScrollWindow.verticalNormalizedPosition = 0;
    }

    // Method to update the currently selected item in the inventory
    public void ShowDetails(int index)
    {
        InventoryItem item = SharedInventory.GetItemDetails(index);
        Debug.Log("Showing: " + item.Name);
        ItemImage.sprite = item.Icon;
        ItemName.text = item.Name;
        ItemDesc.text = item.Description;
        UseItemButton.interactable = true;
        int currentIndex = index;
        UseItemButton.gameObject.SetActive(true);
        if(item.Usable)
        {
            UseItemButton.onClick.RemoveAllListeners();
            UseItemButton.onClick.AddListener(() =>
            {
                SharedInventory.MarkItemAsViewedServerRpc(currentIndex); // show other users that item is viewed
                SharedInventory.UseItem(currentIndex); // perform item activation behavior
                // UIManager.CloseInventory(); // hide the inventory
            });
        }
        else
        {
            UseItemButton.gameObject.SetActive(false);
        }

        if (!item.IsStillNecessary)
        {
            AlreadyViewedLabel.SetActive(true);
            AlreadyViewedText.color = Color.green;
            AlreadyViewedText.text = "Item is no longer needed!";
        }
        else if (item.HasBeenViewed)
        {
            AlreadyViewedLabel.SetActive(true);
            AlreadyViewedText.color = Color.grey;
            AlreadyViewedText.text = "Teammate already used!";
        }
        else
        {
            AlreadyViewedLabel.SetActive(false);
        }
    }

    // Method to add a note to the screen
    public void AddNote()
    {
        SharedNotes.AddNoteForAllServerRpc(new ServerRpcParams());
    }

    public void PlaceNoteInList(GameObject newNote)
    {
        newNote.transform.SetParent(NotesListPanel.transform);
        newNote.transform.localScale = Vector3.one;
    }

    // Switch to the Notes view
    public void ShowNotesTab()
    {
        InventoryPanel.SetActive(false);
        NotesPanel.SetActive(true);

        InventoryButton.interactable = true;
        NotesButton.interactable = false;
    }

    // Switch to the Inventory view
    public void ShowInventoryTab()
    {
        InventoryPanel.SetActive(true);
        NotesPanel.SetActive(false);

        InventoryButton.interactable = false;
        NotesButton.interactable = true;
    }

    // Method to update the sticky note with patient information as clues are completed
    [ClientRpc]
    public void UpdateInformationClientRpc(int clue)
    {
        string output = "Patient Information: \n\n";
        foreach (PatientInformation info in PatientInformationList)
        {
            output += $"\u2022<indent=1em>{info.informationName}: {(info.clue <= clue ? info.value : "?")}</indent>\n\n";
        }
        InformationNote.text = output;
    }

    // Change the sprite shown for the folder depending on what is currently active
    public void ChangeFolderView(bool showingItem)
    {
        if (showingItem)
        {
            FolderImage.sprite = FolderPage1;
            FolderItemPanel.SetActive(true);
            FolderInformationPanel.SetActive(false);
            ShowItemPanelButton.interactable = false;
            ShowInformationPanelButton.interactable = true;
        }
        else
        {
            FolderImage.sprite = FolderPage2;
            FolderItemPanel.SetActive(false);
            FolderInformationPanel.SetActive(true);
            ShowItemPanelButton.interactable = true;
            ShowInformationPanelButton.interactable = false;
        }
    }
}
