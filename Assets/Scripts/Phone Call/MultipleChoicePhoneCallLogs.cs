using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Conversation;
using TMPro;
using UnityEngine.UI;

public class MultipleChoicePhoneCallLogs : NetworkBehaviour
{
    // Team chat history
    [SerializeField] private List<ChatMessage> TeamChatHistory = new List<ChatMessage>();
    [SerializeField] private List<ChatMessage> PhoneChatHistory = new List<ChatMessage>();
    private string ActivePlayerName;
    private bool ControlTaken;

    // Phone conversation data
    [SerializeField] private PrewrittenConversationLine InitialPhoneConversationLine;
    [SerializeField] private PrewrittenConversationLine CurrentPhoneConversationLine;
    [SerializeField] private GameObject ButtonPrefab;

    private void Start()
    {
        ActivePlayerName = FindObjectOfType<PlayerClientData>().GetPlayerName();
        CurrentPhoneConversationLine = InitialPhoneConversationLine;
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
        TeamChatUI teamChatUI = FindObjectOfType<TeamChatUI>();
        teamChatUI.EnablePhone();
        LockPhoneForOthersServerRpc(ActivePlayerName);
        SendTeamChatMessage($"{ActivePlayerName} has taken the speaking role. Only they will be able to speak on the phone.", true);

        // sending starting message
        // fetching the current time
        DateTime currentTime = DateTime.Now;
        string timestamp = currentTime.ToString("HH:mm");
        AddPhoneChatMessageForAllServerRpc("Speaker", timestamp, CurrentPhoneConversationLine.ResponseContent);
    }

    [ServerRpc(RequireOwnership = false)]
    private void LockPhoneForOthersServerRpc(string playerName)
    {
        LockPhoneForOthersClientRpc(playerName);
    }

    [ClientRpc]
    private void LockPhoneForOthersClientRpc(string playerName)
    {
        TeamChatUI teamChatUI = FindObjectOfType<TeamChatUI>();

        // Disable chat for all but player
        if (ActivePlayerName != playerName)
        {
            teamChatUI.DisablePhone();
        }

        // Clear previous chat for all
        teamChatUI.ClearConversation();
    }

    public void SendPhoneChatMessage()
    {
        // clear buttons first
        ClearButtonsServerRpc();

        // fetching the current time
        DateTime currentTime = DateTime.Now;
        string timestamp = currentTime.ToString("HH:mm");

        // fetching the current player name
        string playerName = "Team";

        AddPhoneChatMessageForAllServerRpc(playerName, timestamp, CurrentPhoneConversationLine.PlayerContent);
        AddPhoneChatMessageForAllServerRpc("Speaker", timestamp, CurrentPhoneConversationLine.ResponseContent);

        // win
        if (CurrentPhoneConversationLine.EndState)
        {
            AddPhoneChatMessageForAllServerRpc("System", timestamp, "The call was successfully completed.");

            // LOG HERE
            SaveConversationToLocalFile();

            // enable the proceed to debriefing button
            EnableDebriefButtonForAllServerRpc();
        }
        // fail and continue
        else if (CurrentPhoneConversationLine.FailState)
        {
            AddPhoneChatMessageForAllServerRpc("System", timestamp, CurrentPhoneConversationLine.FailInformation.FailMessage);
            CurrentPhoneConversationLine = CurrentPhoneConversationLine.FailInformation.ReturnPoint;
            // repeat line
            AddPhoneChatMessageForAllServerRpc("Speaker", timestamp, CurrentPhoneConversationLine.ResponseContent);
            // EndConversationForAllServerRpc();
            GenerateButtonsForCurrentLineServerRpc();
        }
        // continue
        else
        {
            GenerateButtonsForCurrentLineServerRpc();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void GenerateButtonsForCurrentLineServerRpc()
    {
        GenerateButtonsForCurrentLineClientRpc();
    }

    [ClientRpc]
    private void GenerateButtonsForCurrentLineClientRpc()
    {
        PlayerInteractions[] players = FindObjectsOfType<PlayerInteractions>();
        foreach (PlayerInteractions player in players)
        {
            if (player.enabled == true)
            {
                GameObject parent = player.GetComponentInChildren<TeamChatUI>().GetInputScrollView();
                foreach (PrewrittenConversationLine line in CurrentPhoneConversationLine.FollowUpOptions)
                {
                    // create button
                    GameObject newButton = Instantiate(ButtonPrefab, parent.transform);
                    newButton.GetComponentInChildren<TMP_Text>().text = line.PlayerContent;
                    newButton.GetComponent<Button>().onClick.AddListener(() => {
                        // TODO: this might desync, check with many players
                        FindObjectOfType<MultipleChoicePhoneCallLogs>().CurrentPhoneConversationLine = line;
                        FindObjectOfType<MultipleChoicePhoneCallLogs>().SendPhoneChatMessage();
                    });
                }
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void ClearButtonsServerRpc()
    {
        ClearButtonsClientRpc();
    }

    [ClientRpc]
    private void ClearButtonsClientRpc()
    {
        PlayerInteractions[] players = FindObjectsOfType<PlayerInteractions>();
        foreach (PlayerInteractions player in players)
        {
            if (player.enabled == true)
            {
                GameObject parent = player.GetComponentInChildren<TeamChatUI>().GetInputScrollView();
                Button[] currentButtons = parent.GetComponentsInChildren<Button>();
                for (int i = 0; i < currentButtons.Length; i++)
                {
                    Destroy(currentButtons[i].gameObject);
                }
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void EnableDebriefButtonForAllServerRpc()
    {
        EnableDebriefButtonForAllClientRpc();
    }

    [ClientRpc]
    private void EnableDebriefButtonForAllClientRpc() 
    {
        PlayerInteractions[] players = FindObjectsOfType<PlayerInteractions>();
        foreach (PlayerInteractions player in players)
        {
            if (player.enabled == true)
            {
                player.GetComponentInChildren<TeamChatUI>().EnableDebriefButton();
            }
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

    // TODO: this is simply for playtesting!
    // should not be used in production environment!
    private void SaveConversationToLocalFile()
    {
        DateTime currentTime = DateTime.Now;
        string filename = currentTime.ToString();
        filename = filename.Replace("/", "-");
        filename = filename.Replace(":", "_");

        string path = Application.persistentDataPath + "/" + filename + ".txt";

        // Debug.Log(path);

        StreamWriter writer = new StreamWriter(path, true);

        foreach(ChatMessage msg in PhoneChatHistory)
        {
            writer.WriteLine($"{msg.PlayerName} ({msg.Timestamp}): {msg.Message.Replace("\n", "")}");
        }

        writer.Close();
        
        // clear PhoneChatHistory
        PhoneChatHistory.Clear();
    }
}