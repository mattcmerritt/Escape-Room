using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class MagnetBoardManager : NetworkBehaviour
{
    public static MagnetBoardManager instance;

    private void Start()
    {
        instance = this;
    }

    [ServerRpc(RequireOwnership = false)]
    public void ChangeMagnetSlotServerRpc(int magnet, int newSlot)
    {
        ChangeMagnetSlotClientRpc(magnet, newSlot);
    }

    [ClientRpc]
    public void ChangeMagnetSlotClientRpc(int magnet, int newSlot)
    {
        // find the player's magnet board
        SingleSideMagnetBoardUI[] magnetBoards = FindObjectsOfType<SingleSideMagnetBoardUI>(true);

        foreach (SingleSideMagnetBoardUI magnetBoard in magnetBoards)
        {
            magnetBoard.MoveMagnetToSlot(magnet, newSlot);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void ChangeMagnetPositionServerRpc(int magnet, float x, float y, float z)
    {
        ChangeMagnetPositionClientRpc(magnet, x, y, z);
    }
    
    [ClientRpc]
    public void ChangeMagnetPositionClientRpc(int magnet, float x, float y, float z)
    {
        // find the player's magnet board
        SingleSideMagnetBoardUI[] magnetBoards = FindObjectsOfType<SingleSideMagnetBoardUI>(true);

        foreach (SingleSideMagnetBoardUI magnetBoard in magnetBoards)
        {
            magnetBoard.MoveMagnetToPosition(magnet, new Vector3(x, y, z));
        }
    }
}
