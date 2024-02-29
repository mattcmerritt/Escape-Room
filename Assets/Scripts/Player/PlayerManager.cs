using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PlayerManager : NetworkBehaviour
{
    public static PlayerManager Instance;

    public string ActivePlayerName;
    [SerializeField] public List<string> PlayersInLobby = new List<string>();

    private void Start()
    {
        Instance = this;
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        ActivePlayerName = FindObjectOfType<PlayerClientData>().GetPlayerName();

        RegisterPlayerServerRpc(ActivePlayerName);
    }

    [ServerRpc(RequireOwnership = false)]
    public void RegisterPlayerServerRpc(string playerName)
    {
        string players = "";
        foreach (string name in PlayersInLobby)
        {
            players += $"{name}|";
        }
        RegisterPlayerClientRpc(playerName, players);
    }

    [ClientRpc]
    public void RegisterPlayerClientRpc(string playerName, string prevPlayers)
    {
        // loading the list from the host / server
        PlayersInLobby = new List<string>(prevPlayers.Split('|'));
        for (int i = 0; i < PlayersInLobby.Count; i++)
        {
            // Debug.Log($"{PlayersInLobby[i]}: {PlayersInLobby[i].Trim()}");
            if (PlayersInLobby[i].Trim() == "")
            {
                PlayersInLobby.Remove(PlayersInLobby[i]);
                i--;
            }
        }
        PlayersInLobby.Add(playerName);
    }
}
