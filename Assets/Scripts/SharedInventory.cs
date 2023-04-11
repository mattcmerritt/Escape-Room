using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

public class SharedInventory : NetworkBehaviour
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
        // adding the item for all other players
        AddItemToAllServerRpc(item.ItemDetails.Name);
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

    [ServerRpc(RequireOwnership = false)]
    private void AddItemToAllServerRpc(string itemName)
    {
        AddItemClientRpc(itemName);
    }

    [ClientRpc]
    private void AddItemClientRpc(string itemName)
    {
        // finding the item in the client's scene
        UtilityObject[] allItems = GameObject.FindObjectsOfType<UtilityObject>();
        foreach (UtilityObject item in allItems)
        {
            if (itemName == item.ItemDetails.Name)
            {
                Items.Add(item);

                InventoryUI inventoryUI = FindObjectOfType<InventoryUI>();
                inventoryUI.AddItem(item);
            }
        }
    }
}
