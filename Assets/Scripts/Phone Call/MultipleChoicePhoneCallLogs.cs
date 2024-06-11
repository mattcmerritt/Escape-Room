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
    [SerializeField] private List<ChatMessage> PhoneChatHistory = new List<ChatMessage>();
    [SerializeField] private string ActivePlayerName;
    [SerializeField] private List<PrewrittenConversationLine> DialogueLines;
    private bool ControlTaken;

    // Phone conversation data
    [SerializeField] private PrewrittenConversationLine InitialPhoneConversationLine;
    [SerializeField] private PrewrittenConversationLine CurrentPhoneConversationLine;
    [SerializeField] private GameObject ButtonPrefab;

    private void Start()
    {
        CurrentPhoneConversationLine = InitialPhoneConversationLine;
    }

    // ------------------------ PHONE CONVERSATION ------------------------
    public void TakePhoneControl(string playerName)
    {
        TeamChatUI teamChatUI = FindObjectOfType<TeamChatUI>();
        teamChatUI.EnablePhone();
        LockPhoneForOthersServerRpc(playerName);

        // sending starting message
        // fetching the current time
        DateTime currentTime = DateTime.Now;
        string timestamp = currentTime.ToString("HH:mm");
        AddPhoneChatMessageForAllServerRpc("Speaker", timestamp, CurrentPhoneConversationLine.ResponseContent);
    }

    [ServerRpc(RequireOwnership = false)]
    private void LockPhoneForOthersServerRpc(string playerName)
    {
        // fetching the current time
        DateTime currentTime = DateTime.Now;
        string timestamp = currentTime.ToString("HH:mm");
        AddPhoneChatMessageForAllServerRpc("<SYSTEM MESSAGE - CLEAR>", timestamp, $"{playerName} has taken the speaking role. Only they will be able to speak on the phone.");
        LockPhoneForOthersClientRpc(playerName);
    }

    [ClientRpc]
    private void LockPhoneForOthersClientRpc(string playerName)
    {
        ActivePlayerName = playerName;

        TeamChatUI teamChatUI = FindObjectOfType<TeamChatUI>();

        // Disable chat for all but player
        if (ActivePlayerName != FindObjectOfType<PlayerClientData>().GetPlayerName())
        {
            teamChatUI.DisablePhone();
        }
    }

    public void PopulateConversationBasedOnCurrentLine()
    {
        // fetching the current time
        DateTime currentTime = DateTime.Now;
        string timestamp = currentTime.ToString("HH:mm");

        // fetching the current player name
        string playerName = "Team";

        // send player message
        string playerMessage = CurrentPhoneConversationLine.PlayerContent;
        if(playerMessage.Contains("<name>"))
        {
            playerMessage = playerMessage.Replace("<name>", ActivePlayerName);
        }
        AddPhoneChatMessageForAllServerRpc(playerName, timestamp, playerMessage);

        // send response (if exists)
        string speakerMessage = CurrentPhoneConversationLine.ResponseContent;
        if(speakerMessage != "")
        {
            if(speakerMessage.Contains("<name>"))
            {
                speakerMessage = speakerMessage.Replace("<name>", ActivePlayerName);
            }
            AddPhoneChatMessageForAllServerRpc("Speaker", timestamp, speakerMessage);
        }

        // win
        if (CurrentPhoneConversationLine.EndState)
        {
            AddPhoneChatMessageForAllServerRpc("<SYSTEM MESSAGE - CLEAR>", timestamp, "The call was successfully completed.");

            GenerateButtonsForCurrentLineServerRpc(); // this will only clear - end state has no new buttons

            // LOG HERE
            SaveConversationToLocalFile();

            // enable the proceed to debriefing button
            EnableDebriefButtonForAllServerRpc();
        }
        // fail and continue
        else if (CurrentPhoneConversationLine.FailState)
        {
            AddPhoneChatMessageForAllServerRpc("<SYSTEM MESSAGE - FAIL>", timestamp, CurrentPhoneConversationLine.FailInformation.FailMessage);
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
    public void UpdateCurrentLineServerRpc(int dialogueLineIndex)
    {
        UpdateCurrentLineClientRpc(dialogueLineIndex);
        CurrentPhoneConversationLine = DialogueLines[dialogueLineIndex];
        PopulateConversationBasedOnCurrentLine(); // send new message after loading it, only if player is the speaker
    }

    [ClientRpc]
    private void UpdateCurrentLineClientRpc(int dialogueLineIndex)
    {
        CurrentPhoneConversationLine = DialogueLines[dialogueLineIndex];
    }

    [ServerRpc(RequireOwnership = false)]
    public void GenerateButtonsForCurrentLineServerRpc()
    {
        GenerateButtonsForCurrentLineClientRpc();
    }

    [ClientRpc]
    private void GenerateButtonsForCurrentLineClientRpc()
    {
        StartCoroutine(CreateButtonsAfterActivePlayerSet());
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

    public IEnumerator CreateButtonsAfterActivePlayerSet()
    {
        // wait for active player to be set first
        yield return new WaitUntil(() => ActivePlayerName != null);

        // find index of current conversation line
        int conversationIndex = DialogueLines.IndexOf(CurrentPhoneConversationLine);

        PlayerInteractions[] players = FindObjectsOfType<PlayerInteractions>();

        // generate new buttons
        foreach (PlayerInteractions player in players)
        {
            if (player.enabled == true)
            {
                GameObject parent = player.GetComponentInChildren<TeamChatUI>().GetInputScrollView();
                foreach (PrewrittenConversationLine line in CurrentPhoneConversationLine.FollowUpOptions)
                {
                    // create button
                    GameObject newButton = Instantiate(ButtonPrefab, parent.transform);
                    newButton.name = $"ConversationChoice-{DialogueLines.IndexOf(CurrentPhoneConversationLine)}";
                    Debug.Log(newButton.name);
                    newButton.GetComponent<RectTransform>().position = new Vector3(220f, newButton.GetComponent<RectTransform>().position.y, newButton.GetComponent<RectTransform>().position.z);

                    string playerMessage = line.PlayerContent;
                    if(playerMessage.Contains("<name>"))
                    {
                        playerMessage = playerMessage.Replace("<name>", ActivePlayerName);
                    }

                    newButton.GetComponentInChildren<TMP_Text>().text = playerMessage;
                    newButton.GetComponent<Button>().onClick.AddListener(() => {
                        FindObjectOfType<MultipleChoicePhoneCallLogs>().UpdateCurrentLineServerRpc(DialogueLines.IndexOf(line));
                    });

                    // disable buttons if not active speaker
                    if(FindObjectOfType<PlayerClientData>().GetPlayerName() != ActivePlayerName)
                    {
                        newButton.GetComponent<Button>().interactable = false;
                    }
                }
            }
        }

        // clear old buttons
        foreach (PlayerInteractions player in players)
        {
            if (player.enabled == true)
            {
                GameObject parent = player.GetComponentInChildren<TeamChatUI>().GetInputScrollView();
                Button[] currentButtons = parent.GetComponentsInChildren<Button>();
                List<string> usedOptions = new List<string>();
                for (int i = 0; i < currentButtons.Length; i++)
                {
                    if(currentButtons[i].gameObject.name != $"ConversationChoice-{conversationIndex}")
                    {
                        Destroy(currentButtons[i].gameObject);
                    }
                    else if(usedOptions.Contains(currentButtons[i].gameObject.GetComponentInChildren<TMP_Text>().text))
                    {
                        Destroy(currentButtons[i].gameObject); // duplicate button, destroy
                    }
                    else if(!usedOptions.Contains(currentButtons[i].gameObject.GetComponentInChildren<TMP_Text>().text))
                    {
                        usedOptions.Add(currentButtons[i].gameObject.GetComponentInChildren<TMP_Text>().text); // add to duplicate list
                    }
                }
            }
        }
    }
}
