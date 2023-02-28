using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SharedInventory : MonoBehaviour
{
    // Inventory state
    [SerializeField] private List<UtilityObject> Items;
    [SerializeField] private List<GameObject> ItemSlots;

    // UI Elements
    [SerializeField] private GameObject CluePanel;
    [SerializeField] private TMP_Text ClueContent;

    private void Start()
    {
        Items = new List<UtilityObject>();
    }

    public void AddItem(UtilityObject item)
    {
        Items.Add(item);
        UpdateUI();
    }

    public void UseItem(int index)
    {
        Debug.Log("Used " + Items[index].ItemDetails.Name);
        Items[index].Used = true;

        if (Items[index].ItemDetails.GetType() == typeof(ClueItem))
        {
            ShowClue((ClueItem) Items[index].ItemDetails);
        }

        UpdateUI();
    }

    public void ShowClue(ClueItem clue)
    {
        PlayerMovement movement = FindObjectOfType<PlayerMovement>();
        movement.LockCamera();
        CluePanel.SetActive(true);
        ClueContent.text = clue.Description;
    }

    public void HideClue()
    {
        PlayerMovement movement = FindObjectOfType<PlayerMovement>();
        movement.UnlockCamera();
        CluePanel.SetActive(false);
    }

    private void UpdateUI()
    {
        // Menu cannot display more than 9 items, cuts off at 9
        if (Items.Count > 9)
        {
            Debug.LogError("Menu out of space!");
        }
        
        // Filling the boxes with the item sprites
        for (int i = 0; i < Items.Count && i < 10; i++)
        {
            GameObject itemIcon = new GameObject("Sprite");
            itemIcon.transform.SetParent(ItemSlots[i].transform);
            Image img = itemIcon.AddComponent<Image>();
            img.sprite = Items[i].Used ? Items[i].ItemDetails.UsedIcon : Items[i].ItemDetails.Icon;
            RectTransform rt = itemIcon.GetComponent<RectTransform>();
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.sizeDelta = Vector2.zero;
            rt.localPosition = Vector3.zero;
        }
    }
}
