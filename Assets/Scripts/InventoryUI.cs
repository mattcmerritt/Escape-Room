using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using Unity.Netcode;

public class InventoryUI : NetworkBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private UIManager UIManager; // the current player's UI manager
    [SerializeField] private bool Focused; // whether the mouse is on the clipboard

    private SharedInventory SharedInventory; // component with actual inventory data

    [SerializeField] private GameObject ItemButtonPrefab; // prefab for adding a new item to the UI
    private List<Button> ItemButtons; // memory for items list

    // UI components to fill
    [SerializeField] private GameObject ItemListBox;
    [SerializeField] private Image ItemImage;
    [SerializeField] private TMP_Text ItemName, ItemDesc;
    [SerializeField] private Button UseItemButton;

    // UI components needed to switch views
    [SerializeField] private GameObject InventoryPanel, NotesPanel;
    [SerializeField] private Button InventoryButton, NotesButton;

    // Information necessary for the notes
    [SerializeField] private TMP_InputField NotesField;
    
    private void Start()
    {
        SharedInventory = FindObjectOfType<SharedInventory>();
        ItemButtons = new List<Button>();
        UseItemButton.interactable = false; // no items can be used at start
    }

    // If a click occurs, defer to the UIManager to see if panel can be closed
    private void Update()
    {
        if (!Focused && Input.GetMouseButtonDown(0))
        {
            UIManager.CloseInventory();
        }

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            NotesField.DeactivateInputField();

            // remove any extra tabs that may have been added onto the end
            NotesField.text = NotesField.text.TrimEnd();
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
            ShowDetails(currentIndex); // when selected, fill menu with details
        });
        TMP_Text label = newBtnObj.GetComponentInChildren<TMP_Text>();
        label.text = item.ItemDetails.Name;
        ItemButtons.Add(newBtn);
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
        UseItemButton.onClick.RemoveAllListeners();
        UseItemButton.onClick.AddListener(() =>
        {
            SharedInventory.UseItem(currentIndex); // perform item activation behavior
            UIManager.CloseInventory(); // hide the inventory
        });
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

    // Update text for all players
    public void UpdateNotesText()
    {
        UpdateNotesTextServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void UpdateNotesTextServerRpc()
    {
        // Debug.Log("Sending to server:\n" + NotesField.text);
        UpdateNotesTextClientRpc(NotesField.text);
    }

    [ClientRpc]
    private void UpdateNotesTextClientRpc(string content)
    {
        // Debug.Log("All clients displaying:\n" + content);
        NotesField.text = content;
    }
}
