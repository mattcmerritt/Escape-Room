using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PhoneCallLogs : NetworkBehaviour
{
    // Team chat history
    [SerializeField] private List<ChatMessage> TeamChatHistory = new List<ChatMessage>();
    [SerializeField] private List<ChatMessage> PhoneChatHistory = new List<ChatMessage>();
    private string ActivePlayerName;
    private bool ControlTaken;

    // Phone conversation data
    [SerializeField] private ConversationLine CurrentPhoneConversationLine;

    private void Start()
    {
        ActivePlayerName = FindObjectOfType<PlayerClientData>().GetPlayerName();
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
        AddTeamChatMessageClientRpc(playerName, timestamp, message, announcement);
    }

    [ClientRpc]
    private void AddTeamChatMessageClientRpc(string playerName, string timestamp, string message, bool announcement)
    {
        Debug.Log($"Client RPC received: {timestamp}, {playerName}: {message}");
        ChatMessage newMessage = new ChatMessage(playerName, timestamp, message);
        TeamChatHistory.Add(newMessage);
        FindObjectOfType<TeamChatUI>(false).AddTeamMessage(newMessage, announcement);
    }

    // ------------------------ PHONE CONVERSATION ------------------------
    public void TakePhoneControl()
    {
        FindObjectOfType<TeamChatUI>().EnablePhone();
        LockPhoneForOthersServerRpc(ActivePlayerName);
        SendTeamChatMessage($"{ActivePlayerName} has taken the speaking role. Only they will be able to speak on the phone.", true);

        // sending starting message
        // fetching the current time
        DateTime currentTime = DateTime.Now;
        string timestamp = currentTime.ToString("HH:mm");
        AddPhoneChatMessageForAllServerRpc("Speaker", timestamp, CurrentPhoneConversationLine.Content);
    }

    [ServerRpc(RequireOwnership = false)]
    private void LockPhoneForOthersServerRpc(string playerName)
    {
        LockPhoneForOthersClientRpc(playerName);
    }

    [ClientRpc]
    private void LockPhoneForOthersClientRpc(string playerName)
    {
        if (ActivePlayerName != playerName)
        {
            FindObjectOfType<TeamChatUI>().DisablePhone();
        }
    }

    public void SendPhoneChatMessage(string message)
    {
        Debug.Log($"Chatlog received message: {message}");

        // fetching the current time
        DateTime currentTime = DateTime.Now;
        string timestamp = currentTime.ToString("HH:mm");

        // fetching the current player name
        string playerName = "Team";

        AddPhoneChatMessageForAllServerRpc(playerName, timestamp, message);

        // attempt to run the conversation from this point
        ConversationLine resultantConversationLine = CurrentPhoneConversationLine.CheckConnectedLines(message);

        if (resultantConversationLine.FailState)
        {
            AddPhoneChatMessageForAllServerRpc("System", timestamp, resultantConversationLine.Content);
        }
        else if (resultantConversationLine.WinState)
        {
            AddPhoneChatMessageForAllServerRpc("Speaker", timestamp, resultantConversationLine.Content);
            AddPhoneChatMessageForAllServerRpc("System", timestamp, "The call was successfully completed.");
        }
        else
        {
            CurrentPhoneConversationLine = resultantConversationLine;
            AddPhoneChatMessageForAllServerRpc("Speaker", timestamp, resultantConversationLine.Content);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void AddPhoneChatMessageForAllServerRpc(string playerName, string timestamp, string message)
    {
        Debug.Log($"Server RPC received: {timestamp}, {playerName}: {message}");
        AddPhoneChatMessageClientRpc(playerName, timestamp, message);
    }

    [ClientRpc]
    private void AddPhoneChatMessageClientRpc(string playerName, string timestamp, string message)
    {
        Debug.Log($"Client RPC received: {timestamp}, {playerName}: {message}");
        ChatMessage newMessage = new ChatMessage(playerName, timestamp, message);
        PhoneChatHistory.Add(newMessage);
        FindObjectOfType<TeamChatUI>(false).AddPhoneMessage(newMessage);
    }
}
