using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ClueEnvelope : UtilityObject
{
    public override void InteractAllClients(PlayerInteractions player)
    {
        base.InteractAllClients(player);

        // updating the clue announcement text at the top of the screen for all players
        TextItemUI clueUI = player.GetComponentInChildren<TextItemUI>(true);
        ClueItem clue = (ClueItem)ItemDetails;
        clueUI.UpdateClue(clue);
    }
}
