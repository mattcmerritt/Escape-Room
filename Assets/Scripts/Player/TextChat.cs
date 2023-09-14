using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

[System.Serializable]
public struct ChatMessage
{
    public string PlayerName;
    public string Timestamp;
    public string Message;

    public ChatMessage(string playerName, string timestamp, string message)
    {
        PlayerName = playerName;
        Timestamp = timestamp;
        Message = message;
    }
}

public class TextChat : NetworkBehaviour
{
    // Chat history
    [SerializeField] private List<ChatMessage> ChatHistory = new List<ChatMessage>();
    private string ActivePlayerName;

    private void Start()
    {
        ActivePlayerName = FindObjectOfType<PlayerClientData>().GetPlayerName();
    }

    public void SendChatMessage(string message)
    {
        Debug.Log($"Chatlog received message: {message}");

        // fetching the current time
        DateTime currentTime = DateTime.Now;
        string timestamp = currentTime.ToString("HH:mm");

        // fetching the current player name
        string playerName = ActivePlayerName;

        AddChatMessageForAllServerRpc(playerName, timestamp, message);
    }

    [ServerRpc(RequireOwnership = false)]
    private void AddChatMessageForAllServerRpc(string playerName, string timestamp, string message)
    {
        Debug.Log($"Server RPC received: {timestamp}, {playerName}: {message}");
        AddChatMessageClientRpc(playerName, timestamp, message);
    }

    [ClientRpc] 
    private void AddChatMessageClientRpc(string playerName, string timestamp, string message)
    {
        Debug.Log($"Client RPC received: {timestamp}, {playerName}: {message}");
        ChatMessage newMessage = new ChatMessage(playerName, timestamp, message);
        ChatHistory.Add(newMessage);
        FindObjectOfType<ChatLogUI>(false).AddMessage(newMessage);
    }
}
