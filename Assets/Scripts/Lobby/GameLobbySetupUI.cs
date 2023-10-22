using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using TMPro;

public class GameLobbySetupUI : MonoBehaviour
{
    [SerializeField] private GameLobby GameLobby;
    [SerializeField] private GameObject MenuParent;
    [SerializeField] private TMP_InputField LobbyCodeInput;
    [SerializeField] private GameObject LobbyListEntry;
    [SerializeField] private GameObject ContentWindow;
    [SerializeField] private GameObject LobbyScreen, ListScreen, JoinedLobbyScreen;
    [SerializeField] private GameObject StartGameButton;
    [SerializeField] private GameObject PlayerListEntryPrefab;
    [SerializeField] private GameObject PlayerContentWindow;

    // Constantly check if the lobbies have marked themselves as started
    // Necessary for the joining players to disable the UI
    // If the lobby is not started, update the player list
    private void Update()
    {
        if (GameLobby.GetStarted())
        {
            CloseUI();
        }
        else
        {
            ListPlayers();
        }
    }

    public void CreateLobby()
    {
        GameLobby.CreateLobby();
        ActivateJoinedLobbyScreen();
    }

    public void JoinLobby()
    {
        GameLobby.JoinLobbyByCode(LobbyCodeInput.text);
        ActivateJoinedLobbyScreen();
    }

    public void JoinLobby(string id)
    {
        GameLobby.JoinLobbyById(id);
        ActivateJoinedLobbyScreen();
    }

    public void StartGame()
    {
        GameLobby.StartGame();
        CloseUI();
    }

    private void CloseUI()
    {
        MenuParent.SetActive(false);
    }

    public void SwitchToLobbyList()
    {
        LobbyScreen.SetActive(false);
        ListScreen.SetActive(true);
    }

    public void ReturnToMainScreen()
    {
        LobbyScreen.SetActive(true);
        ListScreen.SetActive(false);
    }

    public void ActivateJoinedLobbyScreen()
    {
        LobbyScreen.SetActive(false);
        ListScreen.SetActive(false);
        JoinedLobbyScreen.SetActive(true);

        // Only the host should have the option to start the game
        if (GameLobby.IsLobbyHost())
        {
            StartGameButton.SetActive(true);
        }
    }

    public async void ListLobbies()
    {
        // wipe out old entries
        foreach (Transform child in ContentWindow.transform)
        {
            Destroy(child.gameObject);
        }

        List<SimpleLobbyData> lobbies = await GameLobby.ListLobbies();
        foreach (SimpleLobbyData lobby in lobbies)
        {
            GameObject entry = Instantiate(LobbyListEntry, ContentWindow.transform);
            string lobbyId = lobby.Id;
            string lobbyName = lobby.Name;
            int players = lobby.Players;
            int maxPlayers = lobby.MaxPlayers;
            LobbyListEntry.GetComponent<GameLobbyListing>().Initialize(lobbyId, lobbyName, players, maxPlayers);
        }
    }

    public void ListPlayers()
    {
        // wipe out old entries
        foreach (Transform child in PlayerContentWindow.transform)
        {
            Destroy(child.gameObject);
        }

        List<Player> players = GameLobby.ListPlayersInLobby();
        foreach (Player p in players)
        {
            GameObject entry = Instantiate(PlayerListEntryPrefab, PlayerContentWindow.transform);
            if (p.Data["IsObserver"].Value == "true")
            {
                // TODO: update the toggle value to reflect this status
                // TODO: disable the toggle unless if the player is the host player
            }
            // TODO: add listener to call function in GameLobby to update the player data if host player
        } 
    }
}
