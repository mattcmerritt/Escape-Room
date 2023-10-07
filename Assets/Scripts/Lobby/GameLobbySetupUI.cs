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

    public void JoinLobby(string lobbyCode)
    {
        GameLobby.JoinLobby(lobbyCode);
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
        QueryResponse queryResponse = await GameLobby.ListLobbies();
        foreach (Lobby lobby in queryResponse.Results)
        {
            GameObject entry = Instantiate(LobbyListEntry, ContentWindow.transform);
            LobbyListEntry.GetComponent<GameLobbyListing>().Initialize(lobby);
        }
    }
}
