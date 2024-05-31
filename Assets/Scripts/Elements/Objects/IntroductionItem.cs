using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroductionItem : UtilityObject
{
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        StartCoroutine(AddItemOnGameStart());
    }

    public IEnumerator AddItemOnGameStart()
    {
        yield return new WaitUntil(() => PlayerManager.Instance.CheckIfAllPlayersConnected());

        // add the item automatically to the inventory
        SharedInventory sharedInventory = FindObjectOfType<SharedInventory>();
        if (sharedInventory != null && sharedInventory.CheckForItem(ItemDetails.Name) == null)
        {
            sharedInventory.AddItem(this);
        }
        else
        {
            Debug.LogError("Failed to add introduction to inventory");
        }
    }

    public override void Interact(PlayerInteractions player)
    {
        base.Interact(player);

        player.GetComponentInChildren<TextItemUI>(true).UpdateText(((TextItem)ItemDetails).FullDescription);
    }
}
