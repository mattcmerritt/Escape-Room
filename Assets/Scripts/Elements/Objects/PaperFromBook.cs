using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaperFromBook : UtilityObject
{
    public override void Interact(PlayerInteractions player) 
    {
        // open the UI
        base.Interact(player);

        // change the text to match
        player.GetComponentInChildren<TextItemUI>(true).UpdateText(((TextItem) ItemDetails).FullDescription, ItemDetails.Name);
    }
}
