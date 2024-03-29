using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Phone : SimpleObject
{
    // Phone launches the phone UI for all players
    public override void Interact(PlayerInteractions player)
    {
        if (PanelID != "None")
        {
            OpenUIForAllServerRpc();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void OpenUIForAllServerRpc()
    {
        OpenUIForAllClientRpc();
    }

    [ClientRpc]
    private void OpenUIForAllClientRpc() 
    {
        PlayerInteractions[] players = FindObjectsOfType<PlayerInteractions>();
        foreach (PlayerInteractions player in players)
        {
            if (player.enabled == true)
            {
                player.OpenWithUIManager(PanelID);
            }
        }
    }
}
