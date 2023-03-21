using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class InventoryUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private bool Focused, Active;
    private Animator Animator;
    private SharedInventory SharedInventory;

    // Necessary reference to prevent character movement and interaction in the menus
    private PlayerMovement Movement;
    private PlayerInteractions Interactions;
    // Permanently enabled UI objects that should disappear when another menu comes up
    [SerializeField] private GameObject[] PrimaryUI;
    // Prefab for adding a new item to the UI
    [SerializeField] private GameObject ItemButtonPrefab;
    // UI components to fill
    [SerializeField] private GameObject ItemListBox;
    [SerializeField] private Image ItemImage;
    [SerializeField] private TMP_Text ItemName, ItemDesc;
    [SerializeField] private Button UseItemButton;
    // Memory for items list
    private List<Button> ItemButtons;

    private void Start()
    {
        Animator = GetComponent<Animator>();

        Movement = FindObjectOfType<PlayerMovement>();
        Interactions = FindObjectOfType<PlayerInteractions>();

        PrimaryUI = GameObject.FindGameObjectsWithTag("Primary UI");
        SharedInventory = FindObjectOfType<SharedInventory>();

        ItemButtons = new List<Button>();

        UseItemButton.interactable = false;
    }

    private void Update()
    {
        if ((Active && !Focused && Input.GetMouseButtonDown(0)) || Input.GetKeyDown(KeyCode.Tab))
        {
            UpdateUI();
        }

        if (Active)
        {
            Movement.LockCamera();
            Interactions.OpenMenu();
        }
        else
        {
            Movement.UnlockCamera();
            Interactions.CloseMenu();
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Active = true; // changing to true, will be immediately updated to false
            Animator.SetTrigger("Reset");
            UpdateUI();
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

    public void AddItem(UtilityObject item)
    {
        GameObject newBtnObj = Instantiate(ItemButtonPrefab, ItemListBox.transform);
        Button newBtn = newBtnObj.GetComponent<Button>();
        int currentIndex = ItemButtons.Count;
        newBtn.onClick.AddListener(() =>
        {
            ShowDetails(currentIndex);
        });
        TMP_Text label = newBtnObj.GetComponentInChildren<TMP_Text>();
        label.text = item.ItemDetails.Name;
        ItemButtons.Add(newBtn);
    }

    public void ShowDetails(int index)
    {
        InventoryItem item = SharedInventory.GetItemDetails(index);
        // Debug.Log("Showing: " + item.Name);

        // ItemImage, ItemName, ItemDesc, UseItemButton
        ItemImage.sprite = item.Icon;
        ItemName.text = item.Name;
        ItemDesc.text = item.Description;
        UseItemButton.interactable = true;
        int currentIndex = index;
        UseItemButton.onClick.RemoveAllListeners();
        UseItemButton.onClick.AddListener(() =>
        {
            UpdateUI();
            SharedInventory.UseItem(currentIndex);
        });
    }

    private void UpdateUI()
    {
        Animator.SetTrigger("Change");
        Active = !Active;

        foreach (GameObject UI in PrimaryUI)
        {
            if (UI != gameObject)
            {
                UI.SetActive(!Active);
            }
        }
    }
}
