using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ClueEnvelope : UtilityObject
{
    public override void InteractAllClients(PlayerInteractions player)
    {
        // open the ui for all players separately
        Interact(player);
        // updating the clue announcement text at the top of the screen for all players
        TextItemUI clueUI = player.GetComponentInChildren<TextItemUI>(true);
        ClueItem clue = (ClueItem)ItemDetails;
        clueUI.UpdateClue(clue);
    }
}
