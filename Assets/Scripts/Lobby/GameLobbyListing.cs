using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using TMPro;

public class GameLobbyListing : MonoBehaviour
{
    [SerializeField] private TMP_Text LobbyCodeLabel, PlayerCounter;
    [SerializeField] private Button JoinButton;

    [SerializeField] private string LobbyId, LobbyName;
    [SerializeField] private int PlayerCount, MaxPlayers;

    // TODO: FIGURE OUT WHY THE FIRST TIME A LOBBY IS LOADED, THE ENTRY IS EMPTY
    public void Initialize(string id, string name, int players, int maxPlayers)
    {
        LobbyId = id;
        PlayerCount = players;
        MaxPlayers = maxPlayers;

        LobbyCodeLabel.text = id;
        PlayerCounter.text = players + "/" + maxPlayers;

        Debug.Log("<color=white>Lobby List:</color> Join button was given listener for " + id);

        // TODO: FIGURE OUT WHY THIS IS NOT FIRING WHEN THE BUTTON IS PRESSED
        JoinButton.onClick.AddListener(() =>
        {
            Debug.Log("<color=white>Lobby List:</color> Join button was pressed for " + id);
            GameLobbySetupUI gameLobbyUI = FindObjectOfType<GameLobbySetupUI>();
            gameLobbyUI.JoinLobby(id);
        });
    }
}
