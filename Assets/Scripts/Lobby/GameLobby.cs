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
    [SerializeField] private string RelayCode, LobbyCode;
    [SerializeField] private float HeartbeatTimer;
    [SerializeField] private bool HeartbeatActive;

    private async void Start() 
    {
        await UnityServices.InitializeAsync();

        AuthenticationService.Instance.SignedIn += () =>
        {
            Debug.Log("<color=white>Authentication:</color> Signed in " + AuthenticationService.Instance.PlayerId);
        };
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }

    async void Update()
    {
        if (HeartbeatActive)
        {
            HeartbeatTimer -= Time.deltaTime;
            if (HeartbeatTimer <= 0f)
            {
                HeartbeatTimer = 15f;
                Debug.Log("<color=red>Heartbeat:</color> Lobby received a heartbeat.");
                await LobbyService.Instance.SendHeartbeatPingAsync(LobbyCode);
            }
        }
    }

    public async void CreateLobby()
    {
        try
        {
            // creating relay for netcode
            string relayCode = await CreateRelay();
            CreateLobbyOptions createLobbyOptions = new CreateLobbyOptions
            {
                Data = new Dictionary<string, DataObject>
                {
                    { "RelayCode", new DataObject(DataObject.VisibilityOptions.Public, relayCode) }
                }
            };
            // creating lobby
            string lobbyName = "Escape Room";
            int maxPlayers = 4; // TODO: reconfigure this to match proper information
            Lobby lobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayers, createLobbyOptions);
            LobbyCode = lobby.LobbyCode;
            Debug.Log("<color=blue>Lobby:</color> Created Lobby: " + lobby.Name + ", Lobby Code: " + lobby.LobbyCode);
            HeartbeatActive = true;
            HeartbeatTimer = 15f;
        }
        catch (LobbyServiceException e)
        {
            Debug.LogError("<color=blue>Lobby:</color> " + e);
        }
    }

    public async void JoinLobby(string joinCode)
    {
        try
        {
            Lobby lobby = await Lobbies.Instance.JoinLobbyByIdAsync(joinCode);
            string relayCode = lobby.Data["RelayCode"].Value;
            JoinRelay(relayCode);
            LobbyCode = lobby.LobbyCode;
        }
        catch (LobbyServiceException e)
        {
            Debug.LogError("<color=blue>Lobby:</color> " + e);
        }
    }

    public async Task<QueryResponse> ListLobbies()
    {
        try
        {
            QueryResponse queryResponse = await Lobbies.Instance.QueryLobbiesAsync();
            string output = "<color=blue>Lobby:</color> Lobbies found: " + queryResponse.Results.Count + "\n";
            foreach (Lobby lobby in queryResponse.Results)
            {
                output += "\t" + lobby.Name + " " + lobby.MaxPlayers;
            }
            Debug.Log(output);
            return queryResponse;
        }
        catch (LobbyServiceException e)
        {
            Debug.LogError("<color=blue>Lobby:</color> " + e);
            return null;
        }
    }

    public async Task<string> CreateRelay()
    {
        try
        {
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(3);
            string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
            Debug.Log("<color=green>Relay:</color> Created Relay with code: " + joinCode);
            RelayCode = joinCode;
            RelayServerData relayServerData = new RelayServerData(allocation, "dtls");
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);
            // OLD VERSION
            /*
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetHostRelayData(
                allocation.RelayServer.IpV4,
                (ushort)allocation.RelayServer.Port,
                allocation.AllocationIdBytes,
                allocation.Key,
                allocation.ConnectionData
            );
            */
            NetworkManager.Singleton.StartHost();
            return joinCode;
        }
        catch (RelayServiceException e)
        {
            Debug.LogError("<color=green>Relay:</color> " + e);
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
            // OLD VERSION
            /*
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetClientRelayData(
                joinAllocation.RelayServer.IpV4,
                (ushort) joinAllocation.RelayServer.Port,
                joinAllocation.AllocationIdBytes,
                joinAllocation.Key,
                joinAllocation.ConnectionData,
                joinAllocation.HostConnectionData
            );
            */
            NetworkManager.Singleton.StartClient();
        }
        catch (RelayServiceException e)
        {
            Debug.LogError("<color=green>Relay:</color> " + e);
        }
    }

    public string GetCurrentJoinCode()
    {
        return LobbyCode;
    }
}
