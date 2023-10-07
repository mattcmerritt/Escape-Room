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

    public void Initialize(Lobby lobby)
    {
        LobbyCode = lobby.LobbyCode;
        PlayerCount = lobby.Players.Count;
        MaxPlayers = lobby.MaxPlayers;

        LobbyCodeLabel.text = LobbyCode;
        PlayerCounter.text = PlayerCount + "/" + MaxPlayers;
        JoinButton.onClick.AddListener(() =>
        {
            GameLobby gameLobby = FindObjectOfType<GameLobby>();
            gameLobby.JoinRelay(LobbyCode);
        });
    }
}
