using System.Collections;
using System.Collections.Generic;
using Unity.Services.Core;
using Unity.Services.Vivox;
using Unity.Services.Authentication;
using UnityEngine;

// Largely inspired by Unity Game Lobby Sample code, specifically GameManager.cs
// Ref: https://github.com/Unity-Technologies/com.unity.services.samples.game-lobby
public class VoiceChatManager : MonoBehaviour
{
    private VivoxSetup VivoxSetup = new VivoxSetup();
    [SerializeField] private string Key, Issuer, Domain, Server;

    private async void Awake()
    {
        InitializationOptions options = new InitializationOptions();
        if (CheckManualCredentials())
        {
            options.SetVivoxCredentials(Server, Domain, Issuer, Key);
        }
        await UnityServices.InitializeAsync(options);
        if(!CheckManualCredentials())
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }
        VivoxSetup.Initialize();
    }

    private bool CheckManualCredentials()
    {
        return !(string.IsNullOrEmpty(Key) && string.IsNullOrEmpty(Issuer) && string.IsNullOrEmpty(Domain) && string.IsNullOrEmpty(Server));
    }

    public void JoinVoiceChat()
    {
        VivoxSetup.JoinLobbyChannel("");
    }

    private void OnDestroy() 
    {
        VivoxSetup.LeaveLobbyChannel();
        VivoxSetup.Uninitialize();
    }
}
