using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaperFromBook : UtilityObject
{
    public override void Interact(PlayerInteractions player) 
    {
        player.GetComponentInChildren<TextItemUI>(true).UpdateText(((TextItem) ItemDetails).FullDescription);
    }
}
