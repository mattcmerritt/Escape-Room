using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Authentication;
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
    [SerializeField] private GameObject PlayerListEntryPrefab, OtherPlayerListEntryPrefab;
    [SerializeField] private GameObject PlayerContentWindow;
    [SerializeField] private TMP_Text LobbyCode, PlayerCounter;

    private float ListPlayersTimer = 0.75f;
    private bool InitialLoading = true;

    // Constantly check if the lobbies have marked themselves as started
    // Necessary for the joining players to disable the UI
    // If the lobby is not started, update the player list
    private void Update()
    {
        if (AuthenticationService.Instance.IsSignedIn && InitialLoading)
        {
            ListLobbies(); // this will cause the failed to load entries
            InitialLoading = false;
            StartCoroutine(DelayedInitialLoad());
        }
        if (GameLobby.GetStarted())
        {
            CloseUI();
        }
        else
        {
            if (GameLobby.LobbyActive())
            {
                // show the start button if host
                if (GameLobby.IsLobbyHost())
                {
                    StartGameButton.SetActive(true);
                }

                // relisting the players
                ListPlayersTimer -= Time.deltaTime;
                if (ListPlayersTimer <= 0f)
                {
                    ListPlayersTimer = 0.75f;
                    LobbyCode.text = "Code:\n" + GameLobby.GetCurrentJoinCode();
                    PlayerCounter.text = "Players: " + GameLobby.GetCurrentPlayerCount();
                    ListPlayers();
                }
            }
        }
    }

    private IEnumerator DelayedInitialLoad()
    { 
        yield return new WaitForSeconds(1f);
        ListLobbies();

        // continue refreshing on a timer
        yield return new WaitForSeconds(2f);
        StartCoroutine(DelayedInitialLoad());
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
            Debug.Log($"<color=white>Lobby List:</color> Instantiating entry.");
            GameObject entry = Instantiate(LobbyListEntry, ContentWindow.transform);
            Debug.Log($"<color=white>Lobby List:</color> Finished instantiating entry.");
            string lobbyId = lobby.Id;
            string lobbyName = lobby.Name;
            int players = lobby.Players;
            int maxPlayers = lobby.MaxPlayers;
            LobbyListEntry.GetComponent<GameLobbyListing>().Initialize(lobbyId, lobbyName, players, maxPlayers);
            Debug.Log($"<color=white>Lobby List:</color> Finished preparing entry.");
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
            string playerName = p.Data["PlayerName"].Value;
            bool isObserver = p.Data["IsObserver"].Value == "true";
            bool isHost = p.Data["IsHost"].Value == "true";
            string playerId = p.Id;
            bool isYou = playerId == AuthenticationService.Instance.PlayerId;

            GameObject entry;
            if (isYou)
            {
                entry = Instantiate(PlayerListEntryPrefab, PlayerContentWindow.transform);
            }
            else
            {
                entry = Instantiate(OtherPlayerListEntryPrefab, PlayerContentWindow.transform);
            }
            entry.GetComponent<PlayerListingEntry>().Initialize(playerName, isHost, isObserver, playerId, isYou);
        } 
    }
}
