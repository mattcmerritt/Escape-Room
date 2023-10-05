using System.Collections;
using System.Collections.Generic;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;

// Code taken from tutorial found at: https://www.youtube.com/watch?v=-KDlEBfCBiU
public class GameLobby : MonoBehaviour
{
    private async void Start() 
    {
        await UnityServices.InitializeAsync();

        AuthenticationService.Instance.SignedIn += () =>
        {
            Debug.Log("<color=blue>Lobby:</color> Signed in " + AuthenticationService.Instance.PlayerId);
        };
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }

    public async void CreateLobby()
    {
        try
        {
            string lobbyName = "MyLobby";
            int maxPlayers = 4; // TODO: reconfigure this to match proper information
            Lobby lobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayers);

            Debug.Log("<color=blue>Lobby:</color> Created Lobby: " + lobby.Name + ", Lobby Code: " + lobby.LobbyCode);
        }
        catch (LobbyServiceException e)
        {
            Debug.LogError("<color=blue>Lobby:</color> " + e);
        }
    }
}
