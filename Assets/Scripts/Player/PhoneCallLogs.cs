using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Conversation;

public class PhoneCallLogs : NetworkBehaviour
{
    // Team chat history
    [SerializeField] private List<ChatMessage> TeamChatHistory = new List<ChatMessage>();
    [SerializeField] private List<ChatMessage> PhoneChatHistory = new List<ChatMessage>();
    private string ActivePlayerName;
    private bool ControlTaken;

    // OLD - Phone conversation data
    [SerializeField] private ConversationLine InitialPhoneConversationLine;
    private ConversationLine CurrentPhoneConversationLine;

    // Phone conversation data
    [SerializeField] private List<DialogueLine> DialogueLines;
    [SerializeField] private List<KeywordTrigger> KeywordTriggers;
    [SerializeField] private int CurrentPhase;
    [SerializeField] private string GenericFailMessage, GenericSecondFailMessage, PrivacyFailMessage;

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
        TeamChatUI teamChatUI = FindObjectOfType<TeamChatUI>();

        // Disable chat for all but player
        if (ActivePlayerName != playerName)
        {
            teamChatUI.DisablePhone();
        }

        // Clear previous chat for all
        teamChatUI.ClearConversation();
    }

    [ServerRpc(RequireOwnership = false)]
    private void EndConversationForAllServerRpc()
    {
        EndConversationForAllClientRpc();
    }

    [ClientRpc]
    private void EndConversationForAllClientRpc()
    {
        TeamChatUI teamChatUI = FindObjectOfType<TeamChatUI>();
        teamChatUI.ResetPhone();

        // reset conversation state
        foreach (KeywordTrigger kt in KeywordTriggers)
        {
            kt.ResetObject();
        }
        foreach (DialogueLine line in DialogueLines)
        {
            line.ResetObject();
        }
        CurrentPhase = 0; 

        // LOG HERE
        SaveConversationToLocalFile();
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

        DialogueLine triggeredLine = null;
        foreach (DialogueLine line in DialogueLines)
        {
            if ((line.Phase == CurrentPhase || line.CheckOutsidePhase) && line.CheckIfTriggered(message)) 
            {
                // force responses to only work in phase
                if (line.Phase == CurrentPhase)
                {
                    triggeredLine = line;
                }
            }
        }

        if (triggeredLine != null)
        {
            AddPhoneChatMessageForAllServerRpc("Speaker", timestamp, triggeredLine.Content);

            if (triggeredLine.WinState)
            {
                AddPhoneChatMessageForAllServerRpc("System", timestamp, "The call was successfully completed.");

                // LOG HERE
                SaveConversationToLocalFile();
            }

            if(triggeredLine.PhaseTransitionAfter)
            {
                CurrentPhase += 1;
            }

            // can only enter here for privacy fail
            if (triggeredLine.FailState && CurrentPhase == 0)
            {
                AddPhoneChatMessageForAllServerRpc("System", timestamp, PrivacyFailMessage);
                EndConversationForAllServerRpc();
            }
            else if (triggeredLine.FailState)
            {
                AddPhoneChatMessageForAllServerRpc("System", timestamp, "The call ended early. Please try again.");
                EndConversationForAllServerRpc();
            }
        }
        // generic fail - unclear
        else
        {
            if (CurrentPhase == 0)
            {
                AddPhoneChatMessageForAllServerRpc("Speaker", timestamp, GenericFailMessage);
            }
            else
            {
                AddPhoneChatMessageForAllServerRpc("Speaker", timestamp, GenericSecondFailMessage);
            }
            AddPhoneChatMessageForAllServerRpc("System", timestamp, "The call ended early. Please try again.");
            EndConversationForAllServerRpc();
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
