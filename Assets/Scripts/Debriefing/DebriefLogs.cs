using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class DebriefLogs : NetworkBehaviour
{
    [SerializeField] private List<Card> DebriefingCards;
    [SerializeField] private int CardIndex;
    [SerializeField] private bool CardFlipped;

    // Team chat history
    [SerializeField] private List<ChatMessage> TeamChatHistory = new List<ChatMessage>();
    private string ActivePlayerName;
    private List<string> PlayersInLobby;
    private List<string> PlayersWhoHaveSpoken;

    private void Start()
    {
        ActivePlayerName = FindObjectOfType<PlayerClientData>().GetPlayerName();

        RegisterPlayerServerRpc(ActivePlayerName);
        ForceLoadCardServerRpc(0); // could cause issue with late join
    }

    [ServerRpc(RequireOwnership = false)]
    public void RegisterPlayerServerRpc(string playerName)
    {
        RegisterPlayerClientRpc(playerName);
    }

    [ClientRpc]
    public void RegisterPlayerClientRpc(string playerName)
    {
        PlayersInLobby.Add(playerName);
    }

    // ------------------------ TEAM CONVERSATION ------------------------
    public void SendTeamChatMessage(string message, bool announcement)
    {
        Debug.Log($"Chatlog received message: {message}");

        // fetching the current time
        DateTime currentTime = DateTime.Now;
        string timestamp = currentTime.ToString("HH:mm");

        // fetching the current player name
        string playerName = ActivePlayerName;

        AddTeamChatMessageForAllServerRpc(playerName, timestamp, message, announcement);
    }

    [ServerRpc(RequireOwnership = false)]
    private void AddTeamChatMessageForAllServerRpc(string playerName, string timestamp, string message, bool announcement)
    {
        Debug.Log($"Server RPC received: {timestamp}, {playerName}: {message}");

        PlayersWhoHaveSpoken.Add(playerName);

        AddTeamChatMessageClientRpc(playerName, timestamp, message, announcement);
    }

    [ClientRpc]
    private void AddTeamChatMessageClientRpc(string playerName, string timestamp, string message, bool announcement)
    {
        Debug.Log($"Client RPC received: {timestamp}, {playerName}: {message}");
        ChatMessage newMessage = new ChatMessage(playerName, timestamp, message);
        TeamChatHistory.Add(newMessage);
        FindObjectOfType<TeamDebriefUI>(false).AddTeamMessage(newMessage, announcement);
    }

    // ------------------------ CARDS ------------------------
    [ServerRpc(RequireOwnership = false)]
    private void ForceLoadCardServerRpc(int index)
    {
        ForceLoadCardClientRpc(index);
    }

    [ClientRpc]
    private void ForceLoadCardClientRpc(int index)
    {
        FindObjectOfType<TeamDebriefUI>(false).LoadCardContents(DebriefingCards[index]);
    }

    [ServerRpc(RequireOwnership = false)]
    public void FlipCardServerRpc()
    {
        if (PlayersInLobby.Count == PlayersWhoHaveSpoken.Count)
        {
            FlipCardClientRpc();
        }
    }
    
    [ClientRpc]
    public void FlipCardClientRpc()
    {
        Debug.Log($"Flip Card Client RPC received");

        // if card is on question, flip
        if (!CardFlipped)
        {
            CardFlipped = true;
            FindObjectOfType<TeamDebriefUI>(false).FlipCard();
        }
        else
        {
            CardIndex++;
            CardFlipped = false;
            if (CardIndex < DebriefingCards.Count)
            {
                FindObjectOfType<TeamDebriefUI>(false).LoadCardContents(DebriefingCards[CardIndex]);
            } 
            else
            {
                Debug.Log("Debrief completed!");
            }
        }
    }
}
