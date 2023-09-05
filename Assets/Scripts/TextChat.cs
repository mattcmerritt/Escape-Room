using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

[System.Serializable]
public struct ChatMessage
{
    public string PlayerName;
    public string TimeStamp;
    public string Message;

    public ChatMessage(string playerName, string timeStamp, string message)
    {
        PlayerName = playerName;
        TimeStamp = timeStamp;
        Message = message;
    }
}

public class TextChat : NetworkBehaviour
{
    // Chat history
    [SerializeField] private List<ChatMessage> ChatHistory = new List<ChatMessage>();

    // TODO: implement player name and timestamp in a way that does not require it as a parameter
    //       this can be done using instance data or by reading it from other places
    public void SendMessage(string playerName, string timeStamp, string message)
    {
        AddChatMessageForAllServerRpc(playerName, timeStamp, message);
    }

    [ServerRpc(RequireOwnership = false)]
    private void AddChatMessageForAllServerRpc(string playerName, string timeStamp, string message)
    {
        AddChatMessageClientRpc(playerName, timeStamp, message);
    }

    [ClientRpc] 
    private void AddChatMessageClientRpc(string playerName, string timeStamp, string message)
    {
        ChatMessage newMessage = new ChatMessage(playerName, timeStamp, message);
        ChatHistory.Add(newMessage);
    }
}
