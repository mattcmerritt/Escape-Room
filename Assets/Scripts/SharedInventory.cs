using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SharedInventory : MonoBehaviour
{
    // Inventory state
    [SerializeField] private List<UtilityObject> Items;
    private InventoryUI InventoryUI;

    private void Start()
    {
        Items = new List<UtilityObject>();

        InventoryUI = FindObjectOfType<InventoryUI>();
    }

    public void AddItem(UtilityObject item)
    {
        Items.Add(item);
        InventoryUI.AddItem(item);
    }

    public void UseItem(int index)
    {
        Debug.Log("Used " + Items[index].ItemDetails.Name);
        Items[index].Used = true;

        Items[index].Interact();
    }

    public InventoryItem GetItemDetails(int index)
    {
        return Items[index].ItemDetails;
    }

    public UtilityObject CheckForItem(string itemName)
    {
        foreach (UtilityObject obj in Items)
        {
            if (obj.ItemDetails.Name == itemName)
            {
                return obj;
            }
        }

        return null;
    }
}
