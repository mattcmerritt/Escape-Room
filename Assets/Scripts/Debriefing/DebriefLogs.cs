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
    [SerializeField] private HashSet<string> PlayersWhoHaveSpoken = new HashSet<string>();

    // Debug
    [SerializeField] private TeamDebriefUI TeamDebriefUI;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        // TODO: figure out how to set the first card 
        //  currently happens too early
        ForceLoadCardServerRpc(0); // could cause issue with late join

        InitialLoadNamesServerRpc(); // could cause issue with late join
    }

    // ------------------------ TEAM CONVERSATION ------------------------
    public void SendTeamChatMessage(string message, bool announcement)
    {
        Debug.Log($"Chatlog received message: {message}");

        // fetching the current time
        DateTime currentTime = DateTime.Now;
        string timestamp = currentTime.ToString("HH:mm");

        // fetching the current player name
        string playerName = PlayerManager.Instance.ActivePlayerName;

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
        PlayersWhoHaveSpoken.Add(playerName);
        Debug.Log($"Client RPC received: {timestamp}, {playerName}: {message}");
        ChatMessage newMessage = new ChatMessage(playerName, timestamp, message);
        TeamChatHistory.Add(newMessage);
        FindObjectOfType<TeamDebriefUI>(false).AddTeamMessage(newMessage, announcement);

        // create list of remaining speakers
        InitialLoadNamesServerRpc();
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
        StartCoroutine(LoadCardWhenUIAvailable(index));
    }

    private IEnumerator LoadCardWhenUIAvailable(int index)
    {
        yield return new WaitUntil(() => FindObjectOfType<TeamDebriefUI>(false) != null);
        FindObjectOfType<TeamDebriefUI>(false).LoadCardContents(DebriefingCards[index]);
    }

    [ServerRpc(RequireOwnership = false)]
    public void FlipCardServerRpc()
    {
        if (PlayerManager.Instance.PlayersInLobby.Count == PlayersWhoHaveSpoken.Count)
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
                PlayersWhoHaveSpoken = new HashSet<string>();
            } 
            else
            {
                Debug.Log("Debrief completed!");

                // end the debrief and go to the finish screen
                SwitchToFinishForAllServerRpc();
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void SwitchToFinishForAllServerRpc()
    {
        SequenceManager.Instance.EndGame();
        SwitchToFinishForAllClientRpc();
    }

    [ClientRpc]
    private void SwitchToFinishForAllClientRpc()
    {
        PlayerInteractions[] players = FindObjectsOfType<PlayerInteractions>();
        foreach (PlayerInteractions player in players)
        {
            if (player.enabled == true)
            {
                player.GetUIManager().CloseUI(player.GetUIManager().GetActiveUIPanel().GetComponent<UIPanel>());
                player.OpenWithUIManager("Finish");
            }
        }
    }

    [ServerRpc(RequireOwnership=false)]
    public void InitialLoadNamesServerRpc()
    {
        InitialLoadNamesClientRpc();
    }

    [ClientRpc]
    public void InitialLoadNamesClientRpc()
    {
        StartCoroutine(UpdateNamesWhenAvailable());
    }

    private IEnumerator UpdateNamesWhenAvailable()
    {
        Debug.Log("Updating remaining speakers in coroutine.");
        yield return new WaitUntil(() => FindObjectOfType<TeamDebriefUI>(false) != null);
        // create a list of people who haven't spoken
        List<string> remainingSpeakers = new List<string>();
        foreach (string name in PlayerManager.Instance.PlayersInLobby)
        {
            if (!PlayersWhoHaveSpoken.Contains(name))
            {
                remainingSpeakers.Add(name);
            }
        }
        TeamDebriefUI = FindObjectOfType<TeamDebriefUI>(false);
        TeamDebriefUI.UpdateRemainingSpeakers(remainingSpeakers);
        Debug.Log("Finished updating remaining speakers in coroutine.");
    }

    public List<ChatMessage> GetChatMessages()
    {
        return TeamChatHistory;
    }
}
