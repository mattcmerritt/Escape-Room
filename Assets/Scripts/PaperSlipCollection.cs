using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

[System.Serializable]
public struct PaperSlipData
{
    public string Letters;
    public string Note;

    public PaperSlipData(string letters, string note)
    {
        Letters = letters;
        Note = note;
    }
}

public class PaperSlipCollection : UtilityObject
{
    [SerializeField] private List<PaperSlipData> PapersCollected = new List<PaperSlipData>();

    public void AddPaperToCollection(string letters, string note)
    {
        AddPaperForAllServerRpc(letters, note);
    }

    [ServerRpc(RequireOwnership = false)]
    private void AddPaperForAllServerRpc(string letters, string note)
    {
        // if this is the first paper, add the collection to the inventory
        if (PapersCollected.Count == 0)
        {
            SharedInventory inventory = GameObject.FindObjectOfType<SharedInventory>();
            inventory.AddItem(this);
        }

        AddPaperClientRpc(letters, note);
    }

    [ClientRpc]
    private void AddPaperClientRpc(string letters, string note)
    {
        PaperSlipData paper = new PaperSlipData(letters, note);
        PapersCollected.Add(paper);

        // finding the correct player
        PlayerInteractions player = null;
        foreach (PlayerInteractions potentialPlayer in FindObjectsOfType<PlayerInteractions>())
        {
            if (potentialPlayer.IsOwner) player = potentialPlayer;
        }

        // updating the player's UI to have the paper
        PaperSlipsUI papersUI = player.GetComponentInChildren<PaperSlipsUI>(true);
        papersUI.AddSlip(letters, note);
    }
}
