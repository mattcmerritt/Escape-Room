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
    [SerializeField] private GameObject LobbyScreen, ListScreen;

    public void CreateLobby()
    {
        // GameLobby.CreateLobby();
        GameLobby.CreateLobby();
        CloseUI();
    }

    public void JoinLobby()
    {
        GameLobby.JoinLobby(LobbyCodeInput.text);
        CloseUI();
    }

    public void JoinLobby(string id)
    {
        GameLobby.JoinLobby(id);
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
            LobbyListEntry.GetComponent<GameLobbyListing>().Initialize(lobbyName, lobbyId, players, maxPlayers);
        }
    }
}
