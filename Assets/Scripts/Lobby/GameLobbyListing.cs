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

    [SerializeField] private string LobbyCode;
    [SerializeField] private int PlayerCount, MaxPlayers;

    public void Initialize(string lobbyCode, int players, int maxPlayers)
    {
        LobbyCode = lobbyCode;
        PlayerCount = players;
        MaxPlayers = maxPlayers;

        LobbyCodeLabel.text = lobbyCode;
        PlayerCounter.text = players + "/" + maxPlayers;
        JoinButton.onClick.AddListener(() =>
        {
            Debug.Log("<color=white>Lobby List:</color> Join button was pressed for " + lobbyCode);
            GameLobbySetupUI gameLobbyUI = FindObjectOfType<GameLobbySetupUI>();
            gameLobbyUI.JoinLobby(lobbyCode);
        });
    }
}
