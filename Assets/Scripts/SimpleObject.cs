using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public abstract class SimpleObject : NetworkBehaviour
{
    [SerializeField] protected string PanelID; // ID that refers the UI manager to open the correct panel

    protected virtual void Start()
    {
        // currently this does not need to do anything special
    }

    // Function for when the player clicks on or uses the object
    public virtual void Interact(PlayerInteractions player)
    {
        if (PanelID != "None")
        {
            player.OpenWithUIManager(PanelID);
        }
    }
}
