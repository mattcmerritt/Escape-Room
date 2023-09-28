using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Services.Authentication;
using Unity.Services.Vivox;
using VivoxUnity;

// Largely inspired by Unity Game Lobby Sample code, specifically VivoxSetup.cs
// Ref: https://github.com/Unity-Technologies/com.unity.services.samples.game-lobby
public class VivoxSetup
{
    private bool HasInitialized = false;
    private ILoginSession LoginSession = null;
    private IChannelSession ChannelSession = null;

    // Initialize the Vivox service, before actually joining any audio channels.
    public void Initialize() 
    {
        // Assume that we are not already initializing
        VivoxService.Instance.Initialize();
        Account account = new Account(AuthenticationService.Instance.PlayerId);
        LoginSession = VivoxService.Instance.Client.GetLoginSession(account);
        string token = LoginSession.GetLoginToken();

        LoginSession.BeginLogin(token, SubscriptionMode.Accept, null, null, null, result => 
        {
            try
            {
                LoginSession.EndLogin(result);
                HasInitialized = true;
            }
            catch (Exception ex) 
            {
                Debug.LogWarning("Vivox initialization failed! Login failed: " + ex.Message);
            }
        });
    }

    // Start joining a voice channel for that lobby. Be sure to complete Initialize first.
    public void JoinLobbyChannel(string lobbyId) 
    {
        if (!HasInitialized || LoginSession.State != LoginState.LoggedIn)
        {
            Debug.Log("Not logged in to Vivox, cannot join an audio channel.");
            return;
        }

        ChannelType channelType = ChannelType.NonPositional;
        Channel channel = new Channel(lobbyId + "_voice", channelType, null);
        ChannelSession = LoginSession.GetChannelSession(channel);
        string token = ChannelSession.GetConnectToken();

        ChannelSession.BeginConnect(true, false, true, token, result =>
        {
            try
            {
                // Special case: It's possible for the player to leave the lobby between the time we called BeginConnect and the time we hit this callback.
                // If that's the case, we should abort the rest of the connection process.
                if (ChannelSession.ChannelState == ConnectionState.Disconnecting ||
                    ChannelSession.ChannelState == ConnectionState.Disconnected)
                {
                    UnityEngine.Debug.LogWarning("Vivox channel is already disconnecting. Terminating the channel connect sequence.");
                    ChannelId id = ChannelSession.Channel;
                    ChannelSession?.Disconnect(
                        (result) => 
                        {
                            LoginSession.DeleteChannelSession(id);
                            ChannelSession = null;
                        }
                    );
                    return;
                }

                ChannelSession.EndConnect(result);
            }
            catch (Exception ex) 
            {
                Debug.LogWarning("Vivox failed to connect: " + ex.Message);
                ChannelSession?.Disconnect();
            }
        });
    }

    // Leave a lobby.
    public void LeaveLobbyChannel()
    {
        if (ChannelSession != null)
        {
            // Special case: The EndConnect call requires a little bit of time before the connection actually completes, but the player might
            // disconnect before then. If so, sending the Disconnect now will fail, and the played would stay connected to voice while no longer
            // in the lobby. So, we will need to wait until the connection is completed before disconnecting in that case.
            if (ChannelSession.ChannelState == ConnectionState.Connecting)
            {
                Debug.LogWarning("Vivox channel is currently connecting, could not disconnect.");
            }

            ChannelId id = ChannelSession.Channel;
            ChannelSession?.Disconnect(
                (result) =>
                {
                    LoginSession.DeleteChannelSession(id);
                    ChannelSession = null;
                }
            );
        }
    }

    // Disconnects the player from Vivox completely.
    public void Uninitialize()
    {
        if (!HasInitialized) {
            return;
        }
        LoginSession.Logout();
    }
}
