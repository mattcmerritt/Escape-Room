using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MongoDB.Driver;

public class DatabaseExporter : MonoBehaviour
{
    [System.Serializable]
    private class ChatMessageData
    {
        public string sender;
        public string message;
    }

    [System.Serializable]
    private class ExportData
    {
        public List<string> playerNames;
        public List<ChatMessageData> messageHistory;
        public List<ChatMessageData> debriefHistory;
    }

    private string connectionString;
    private static IMongoCollection<ExportData> collection;

    public void ConfigureDatabaseConnection()
    {
        // generate database connection
        Secret[] secrets = Resources.LoadAll<Secret>("Secrets");
        Secret secret = Array.Find<Secret>(secrets, (Secret s) => s.name.Contains("Database"));
        connectionString = secret.Content;

        //Create MongoDB client
        MongoClient client = new MongoClient(connectionString);
        IMongoDatabase database = client.GetDatabase("results");
        collection = database.GetCollection<ExportData>("escapes");

        Debug.Log("<color=yellow>Exporter:</color> Database was configured for export.");
    }

    public void RecordResults()
    {
        // gather chat data from game objects
        TextChat textChat = FindObjectOfType<TextChat>();
        DebriefLogs debrief = FindObjectOfType<DebriefLogs>();
        PlayerManager playerManager = FindObjectOfType<PlayerManager>();

        List<ChatMessageData> chatMessageHistory = new List<ChatMessageData>();
        List<ChatMessageData> debriefMessageHistory = new List<ChatMessageData>();
        List<string> players = playerManager.ListPlayersInLobby();

        foreach (ChatMessage chatMessage in textChat.GetChatMessages())
        {
            ChatMessageData messageData = new ChatMessageData() { sender = chatMessage.PlayerName, message = chatMessage.Message };
            chatMessageHistory.Add(messageData);
        }

        foreach (ChatMessage chatMessage in debrief.GetChatMessages())
        {
            ChatMessageData messageData = new ChatMessageData()
            {
                sender = chatMessage.PlayerName,
                message = chatMessage.Message
            };
            debriefMessageHistory.Add(messageData);
        }

        ExportData exportData = new ExportData()
        {
            playerNames = players,
            messageHistory = chatMessageHistory,
            debriefHistory = debriefMessageHistory,
        };

        collection.InsertOne(exportData);
        Debug.Log("<color=yellow>Exporter:</color> The game data has been saved to the database.");
    }
}
