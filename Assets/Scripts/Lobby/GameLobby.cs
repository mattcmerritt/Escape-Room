using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using UnityEngine;

// Code taken from tutorial found at: https://www.youtube.com/watch?v=-KDlEBfCBiU
public class GameLobby : MonoBehaviour
{
    private Lobby ActiveLobby;
    [SerializeField] private string RelayCode, LobbyCode;
    [SerializeField] private float HeartbeatTimer, LobbyUpdateTimer;
    [SerializeField] private bool HeartbeatActive;
    [SerializeField] private bool Started;
    [SerializeField] private string PlayerName;
    [SerializeField] private int PlayerCount = 5;

    private async void Start() 
    {
        await UnityServices.InitializeAsync();

        AuthenticationService.Instance.SignedIn += () =>
        {
            Debug.Log("<color=white>Authentication:</color> Signed in " + AuthenticationService.Instance.PlayerId);
        };
        await AuthenticationService.Instance.SignInAnonymouslyAsync();

        // Fetching the player data
        PlayerClientData playerData = FindObjectOfType<PlayerClientData>();
        PlayerName = playerData.GetPlayerName();
    }

    async void Update()
    {
        if (HeartbeatActive)
        {
            HeartbeatTimer -= Time.deltaTime;
            if (HeartbeatTimer <= 0f)
            {
                HeartbeatTimer = 15f;
                Debug.Log("<color=red>Heartbeat:</color> Lobby " + ActiveLobby.LobbyCode + " received a heartbeat.");
                await LobbyService.Instance.SendHeartbeatPingAsync(ActiveLobby.Id);
            }
        }
        else
        {
            // find the current player, if marked as host and heartbeat is not active, set heartbeat to active
            Player player = GetCurrentPlayer();
            if (player != null && player.Data["IsHost"].Value == "true")
            {
                HeartbeatActive = true;
            }
        }

        // checking for lobby updates every half second
        if (ActiveLobby != null && !Started)
        {
            LobbyUpdateTimer -= Time.deltaTime;
            if (LobbyUpdateTimer <= 0f)
            {
                LobbyUpdateTimer = 1f;
                Debug.Log("<color=blue>Lobby:</color> Updating lobby data...");
                CheckForLobbyUpdates();
            }
        }
    }

    // ------ METHODS FOR THE HOSTING PLAYERS ------
    public async void CreateLobby()
    {
        try
        {
            // creating relay for netcode
            CreateLobbyOptions createLobbyOptions = new CreateLobbyOptions
            {
                Player = CreatePlayer(true),
                Data = new Dictionary<string, DataObject>
                {
                    { "RelayCode", new DataObject(DataObject.VisibilityOptions.Public, "") },
                    { "GameStarted", new DataObject(DataObject.VisibilityOptions.Public, "0") }
                }
            };
            // creating lobby
            PlayerClientData playerData = FindObjectOfType<PlayerClientData>();
            string lobbyName = $"{playerData.GetPlayerName()}'s Room";
            int maxPlayers = PlayerCount; // If more spots needed (like an observer), reconfigure this to match proper information
            Lobby lobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayers, createLobbyOptions);
            LobbyCode = lobby.LobbyCode;
            ActiveLobby = lobby;
            Debug.Log("<color=blue>Lobby:</color> Created Lobby: " + lobby.Name + ", Lobby Code: " + lobby.LobbyCode);
            HeartbeatActive = true;
            HeartbeatTimer = 15f;
            // UI should acctivate here
        }
        catch (LobbyServiceException e)
        {
            Debug.LogError("<color=blue>Lobby:</color> " + e);
        }
    }

    public async Task<string> CreateRelay()
    {
        try
        {
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(PlayerCount - 1); // If more spots needed (like an observer), reconfigure this to match proper information
            string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
            Debug.Log("<color=green>Relay:</color> Created Relay with code: " + joinCode);
            RelayCode = joinCode;
            RelayServerData relayServerData = new RelayServerData(allocation, "dtls");
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);
            NetworkManager.Singleton.StartHost();
            return joinCode;
        }
        catch (RelayServiceException e)
        {
            Debug.LogError("<color=green>Relay:</color> " + e);
            return null;
        }
    }

    // Note: only the host should have this value defined
    public bool IsLobbyHost()
    {
        return GetCurrentPlayer() != null && GetCurrentPlayer().Data["IsHost"].Value == "true";
    }

    public async void StartGame()
    {
        try
        {
            if (IsLobbyHost())
            {
                Debug.Log("<color=blue>Lobby:</color> Host starting lobby.");

                string relayCode = await CreateRelay();
                Lobby lobby = await Lobbies.Instance.UpdateLobbyAsync(ActiveLobby.Id, new UpdateLobbyOptions
                {
                    Data = new Dictionary<string, DataObject>
                    {
                        { "GameStarted", new DataObject(DataObject.VisibilityOptions.Public, "1") },
                        { "RelayCode", new DataObject(DataObject.VisibilityOptions.Public, relayCode) }
                    }
                });

                ActiveLobby = lobby;
                Started = true;
            }
        }
        catch (LobbyServiceException e)
        {
            Debug.LogError("<color=blue>Lobby:</color> " + e);
        }
    }

    // ------ METHODS FOR THE JOINING PLAYERS ------
    public async void JoinLobbyById(string id)
    {
        try
        {
            JoinLobbyByIdOptions joinLobbyByIdOptions = new JoinLobbyByIdOptions
            {
                Player = CreatePlayer(false)
            };
            Lobby lobby = await Lobbies.Instance.JoinLobbyByIdAsync(id, joinLobbyByIdOptions);
            LobbyCode = lobby.LobbyCode;
            ActiveLobby = lobby;
            // UI should acctivate here
        }
        catch (LobbyServiceException e)
        {
            Debug.LogError("<color=blue>Lobby:</color> " + e);
        }
    }

    public async void JoinLobbyByCode(string code)
    {
        try
        {
            JoinLobbyByCodeOptions joinLobbyByCodeOptions = new JoinLobbyByCodeOptions
            {
                Player = CreatePlayer(false)
            };
            Lobby lobby = await Lobbies.Instance.JoinLobbyByCodeAsync(code, joinLobbyByCodeOptions);
            LobbyCode = lobby.LobbyCode;
            ActiveLobby = lobby;
            // UI should acctivate here
        }
        catch (LobbyServiceException e)
        {
            Debug.LogError("<color=blue>Lobby:</color> " + e);
        }
    }

    public async Task<bool> LeaveLobby()
    {
        try
        {
            Player player = GetCurrentPlayer();
            string oldLobbyId = ActiveLobby.Id;

            // clean up for script to be reused
            ActiveLobby = null;
            RelayCode = null;
            LobbyCode = null;
            HeartbeatTimer = 0f;
            LobbyUpdateTimer = 0f;
            HeartbeatActive = false;
            Started = false;

            // having the original player leave the lobby
            await LobbyService.Instance.RemovePlayerAsync(oldLobbyId, player.Id);

            Debug.Log("<color=blue>Lobby:</color> Successfully disconnected from lobby.");

            return true;
        }
        catch (LobbyServiceException e)
        {
            Debug.LogError("<color=blue>Lobby:</color> " + e);

            return false;
        }
    }

    public async Task<List<SimpleLobbyData>> ListLobbies()
    {
        try
        {
            // parameters to only see lobbies with open spots that have not started
                QueryLobbiesOptions queryLobbyOptions = new QueryLobbiesOptions
                {
                    Filters = new List<QueryFilter>
                    {
                        new QueryFilter(QueryFilter.FieldOptions.AvailableSlots, "0", QueryFilter.OpOptions.GT),
                        // new QueryFilter(QueryFilter.FieldOptions.S1, "0", QueryFilter.OpOptions.EQ)
                    }
                };

            QueryResponse queryResponse = await Lobbies.Instance.QueryLobbiesAsync(queryLobbyOptions);
            string output = "<color=blue>Lobby:</color> Lobbies found: " + queryResponse.Results.Count + "\n";
            List<SimpleLobbyData> lobbies = new List<SimpleLobbyData>();
            foreach (Lobby lobby in queryResponse.Results)
            {
                output += "\t" + lobby.Name + " " + lobby.Id + " " + lobby.Players.Count + "/" + lobby.MaxPlayers;
                lobbies.Add(new SimpleLobbyData(lobby.Id, lobby.Name, lobby.Players.Count, lobby.MaxPlayers));
            }
            Debug.Log(output);
            return lobbies;
        }
        catch (LobbyServiceException e)
        {
            Debug.LogError("<color=blue>Lobby:</color> " + e);
            return null;
        }
    }

    public async void JoinRelay(string joinCode)
    {
        try
        {
            Debug.Log("<color=green>Relay:</color> Joining Relay with code " + joinCode);
            JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);
            RelayCode = joinCode;
            RelayServerData relayServerData = new RelayServerData(joinAllocation, "dtls");
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);
            NetworkManager.Singleton.StartClient();
        }
        catch (RelayServiceException e)
        {
            Debug.LogError("<color=green>Relay:</color> " + e);
        }
    }

    private async void CheckForLobbyUpdates()
    {
        Lobby updatedLobby = await LobbyService.Instance.GetLobbyAsync(ActiveLobby.Id);
        if (updatedLobby.Data["GameStarted"].Value == "1" && !Started)
        {
            JoinRelay(updatedLobby.Data["RelayCode"].Value);
            Started = true;
        }
        ActiveLobby = updatedLobby;

        // iterate through the lobby and determine if there is still a host
        List<Player> players = ListPlayersInLobby();
        if (players.Count > 0)
        {
            bool foundHost = false;
            foreach (Player p in players)
            {
                if (p.Data["IsHost"].Value == "true")
                {
                    foundHost = true;
                }
            }

            // if no host is found, choose a player to be the new host
            // in case if the order is different on each client, the host will be the player with the lowest id
            if (!foundHost)
            {
                Player newHost = players[0];
                foreach (Player p in players)
                {
                    if (p.Id.CompareTo(newHost.Id) < 0)
                    {
                        newHost = p;
                    }
                }

                // if this instance is the host, do host setup
                if (newHost.Id == GetCurrentPlayer().Id)
                {
                    Debug.Log("<color=blue>Lobby:</color> This player has now been selected to be the new host.");

                    // change player to host
                    updatedLobby = await LobbyService.Instance.UpdatePlayerAsync(ActiveLobby.Id, newHost.Id, new UpdatePlayerOptions
                    {
                        Data = new Dictionary<string, PlayerDataObject>
                        {
                            { "PlayerName", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, newHost.Data["PlayerName"].Value) },
                            { "IsObserver", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, newHost.Data["IsObserver"].Value) },
                            { "IsHost", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Public, "true") }
                        }
                    });

                    ActiveLobby = updatedLobby;
                }
            }
        }
    }

    // ------ PLAYER MANAGEMENT METHODS ------
    public List<Player> ListPlayersInLobby()
    {
        return ActiveLobby.Players;
    }

    private Player CreatePlayer(bool isHost)
    {
        return new Player
        {
            Data = new Dictionary<string, PlayerDataObject>
            {
                { "PlayerName", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, PlayerName) },
                { "IsObserver", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, "false") },
                { "IsHost", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Public, isHost ? "true" : "false") }
            }
        };
    }

    // Assumes that changes can only be made to the current player who is running this script
    public async void UpdatePlayerObserverStatus(bool newValue)
    {
        try
        {
            Lobby updatedLobby = await LobbyService.Instance.UpdatePlayerAsync(ActiveLobby.Id, AuthenticationService.Instance.PlayerId, new UpdatePlayerOptions
            {
                Data = new Dictionary<string, PlayerDataObject>
                {
                    { "PlayerName", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, PlayerName) },
                    { "IsObserver", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, newValue ? "true" : "false") },
                    { "IsHost", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Public, IsLobbyHost() ? "true" : "false") }
                }
            });
            // updating the lobby
            ActiveLobby = updatedLobby;
        } catch (LobbyServiceException e)
        {
            Debug.LogError("<color=blue>Lobby:</color> " + e);
        }
    }

    private Player GetCurrentPlayer()
    {
        if (ActiveLobby == null) return null;

        foreach (Player player in ActiveLobby.Players)
        {
            if (player.Id == AuthenticationService.Instance.PlayerId)
            {
                return player;
            }
        }
        return null; // player could not be found
    }

    // ------ BONUS METHODS FOR AFTER GAME START ------
    public string GetCurrentJoinCode()
    {
        return LobbyCode;
    }

    public string GetCurrentPlayerCount()
    {
        return ActiveLobby.Players.Count + "/" + ActiveLobby.MaxPlayers;
    }

    public int GetCurrentPlayerCountNumber()
    {
        return ActiveLobby.Players.Count;
    }

    public bool GetStarted()
    {
        return Started;
    }

    public bool LobbyActive()
    {
        return ActiveLobby != null;
    }
}
