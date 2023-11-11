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

    [SerializeField] private bool ButtonLoaded;
    private GameLobbySetupUI GameLobbyUI;

    private void Start()
    {
        GameLobbyUI = FindObjectOfType<GameLobbySetupUI>();
        Debug.Log("<color=white>Lobby List:</color> Join button was given listener for " + LobbyId);
        JoinButton.onClick.AddListener(() =>
        {
            if (ButtonLoaded)
            {
                Debug.Log("<color=white>Lobby List:</color> Join button was pressed for " + LobbyId);
                GameLobbyUI.JoinLobby(LobbyId);
            }
        });
        Debug.Log("<color=white>Lobby List:</color> Post button listener add.");
    }

    public void Initialize(string id, string name, int players, int maxPlayers)
    {
        LobbyId = id;
        LobbyName = name;
        PlayerCount = players;
        MaxPlayers = maxPlayers;

        LobbyCodeLabel.text = LobbyName; 
        PlayerCounter.text = PlayerCount + "/" + MaxPlayers;

        JoinButton.interactable = true;

        ButtonLoaded = true;
    }
}
