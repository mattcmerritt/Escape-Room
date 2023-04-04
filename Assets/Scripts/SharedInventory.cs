using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SharedInventory : MonoBehaviour
{
    // Inventory state
    [SerializeField] private List<UtilityObject> Items;

    // All active player inventories
    [SerializeField] private List<InventoryUI> InventoryUIs;

    private void Start()
    {
        Items = new List<UtilityObject>();
        InventoryUIs = new List<InventoryUI>();
    }

    public void AddInventoryUI(InventoryUI inventoryUI)
    {
        InventoryUIs.Add(inventoryUI);
    }

    public void AddItem(UtilityObject item)
    {
        Items.Add(item);
        foreach (InventoryUI inventoryUI in InventoryUIs)
        {
            inventoryUI.AddItem(item);
        }
    }

    public void UseItem(int index, PlayerInteractions player)
    {
        Debug.Log("Used " + Items[index].ItemDetails.Name);
        Items[index].Used = true;

        Items[index].Interact(player);
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
