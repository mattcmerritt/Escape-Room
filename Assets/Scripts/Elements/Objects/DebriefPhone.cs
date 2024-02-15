using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class DebriefPhone : SimpleObject
{
    [ServerRpc(RequireOwnership = false)]
    public void SwitchUIForAllServerRpc()
    {
        SwitchUIForAllClientRpc();
    }

    [ClientRpc]
    private void SwitchUIForAllClientRpc() 
    {
        PlayerInteractions[] players = FindObjectsOfType<PlayerInteractions>();
        foreach (PlayerInteractions player in players)
        {
            if (player.enabled == true)
            {
                player.GetUIManager().CloseUI(player.GetUIManager().GetActiveUIPanel().GetComponent<UIPanel>());
                player.OpenWithUIManager(PanelID);
            }
        }
    }
}
