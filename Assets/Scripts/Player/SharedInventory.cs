using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

public class SharedInventory : NetworkBehaviour
{
    // Inventory state
    [SerializeField] private List<UtilityObject> Items;

    private void Start()
    {
        Items = new List<UtilityObject>();
    }

    public void AddItem(UtilityObject item)
    {
        // adding the item for all other players
        AddItemToAllServerRpc(item.ItemDetails.Name);
    }

    public void UseItem(int index)
    {
        // using the item locally (opening UI)
        // find the client's player
        PlayerInteractions player = null;
        foreach (PlayerInteractions potentialPlayer in FindObjectsOfType<PlayerInteractions>())
        {
            if (potentialPlayer.IsOwner) player = potentialPlayer;
        }
        Items[index].Interact(player);

        if (Items[index] is ClueEnvelope && !Items[index].Used)
        {
            InteractForAllServerRpc(index);
        }

        // using the item for all other players
        UseItemForAllServerRpc(index);
    }

    // special use case for first time opening clues
    // opens the clue on each player
    [ServerRpc(RequireOwnership = false)]
    public void InteractForAllServerRpc(int index)
    {
        InteractForAllClientRpc(index);
    }

    // each client will perform interact on their player
    [ClientRpc]
    public void InteractForAllClientRpc(int index)
    {
        // find the client's player
        PlayerInteractions player = null;
        foreach (PlayerInteractions potentialPlayer in FindObjectsOfType<PlayerInteractions>())
        {
            if (potentialPlayer.IsOwner) player = potentialPlayer;
        }
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

    [ServerRpc(RequireOwnership = false)]
    private void UseItemForAllServerRpc(int index)
    {
        UseItemClientRpc(index);
    }

    [ClientRpc]
    private void UseItemClientRpc(int index)
    {
        Debug.Log("Used " + Items[index].ItemDetails.Name);
        Items[index].Used = true;

        PlayerInteractions player = null;
        foreach (PlayerInteractions potentialPlayer in FindObjectsOfType<PlayerInteractions>())
        {
            if (potentialPlayer.IsOwner) player = potentialPlayer;
        }
        Items[index].InteractAllClients(player);
    }

    public void MarkItemAsViewed(UtilityObject obj)
    {
        int index = Items.FindIndex((UtilityObject util) => util == obj);
        MarkItemAsViewedServerRpc(index);
    }

    [ServerRpc(RequireOwnership = false)]
    public void MarkItemAsViewedServerRpc(int index)
    {
        MarkItemAsViewedClientRpc(index);
    }

    [ClientRpc]
    private void MarkItemAsViewedClientRpc(int index)
    {
        Debug.Log("Player viewed " + Items[index].ItemDetails.Name);
        Items[index].ItemDetails.HasBeenViewed = true;
    }
}
