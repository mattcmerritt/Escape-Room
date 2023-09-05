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

    // TODO: remove testing code
    private void Update() 
    {
        if (Input.GetKeyDown(KeyCode.Return)) 
        {
            SendMessage("Player", "Test Message");
        }
    }


    // TODO: implement player name and timestamp in a way that does not require it as a parameter
    //       this can be done using instance data or by reading it from other places
    public void SendMessage(string playerName, string message)
    {
        // fetching the current time
        DateTime currentTime = DateTime.Now;
        string timestamp = currentTime.ToString("HH:mm");

        Debug.Log($"Time of message: {timestamp}");

        AddChatMessageForAllServerRpc(playerName, timestamp, message);
    }

    [ServerRpc(RequireOwnership = false)]
    private void AddChatMessageForAllServerRpc(string playerName, string timestamp, string message)
    {
        AddChatMessageClientRpc(playerName, timestamp, message);
    }

    [ClientRpc] 
    private void AddChatMessageClientRpc(string playerName, string timestamp, string message)
    {
        ChatMessage newMessage = new ChatMessage(playerName, timestamp, message);
        ChatHistory.Add(newMessage);
    }
}
