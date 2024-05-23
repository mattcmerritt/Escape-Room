using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroductionItem : UtilityObject
{
    protected override void Start()
    {
        base.Start();

        // add the item automatically to the inventory
        SharedInventory sharedInventory = FindObjectOfType<SharedInventory>();
        if (sharedInventory != null && sharedInventory.CheckForItem(ItemDetails.Name) != null)
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
        player.GetComponentInChildren<TextItemUI>(true).UpdateText(((TextItem)ItemDetails).FullDescription);
    }
}
