using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class TeamDebriefUI : MonoBehaviour
{
    [SerializeField] private GameObject MessagePrefab;
    private float MessageX;

    // Team chat elements (functions similarly to the text chat)
    [SerializeField] private RectTransform ChatContentWindow;
    [SerializeField] private TMP_InputField TeamChatbar;

    private float TeamCurrentHeight = 0, TeamLastMessageLocation = 0;
    private EventSystem EventSystem;
    public static string TeamChatlogObjectName;

    // Card elements
    [SerializeField] private TMP_Text CardContentBox;
    private Card CurrentCard;
    // TODO: animate the cards

    private void Start()
    {
        RectTransform prefabMessageTransform = MessagePrefab.GetComponent<RectTransform>();
        MessageX = prefabMessageTransform.anchoredPosition.x;

        EventSystem = FindObjectOfType<EventSystem>();
        TeamChatlogObjectName = TeamChatbar.name;
    }

    // ------------------------ TEAM CONVERSATION ------------------------
    public void SendTeamChatMessage()
    {
        Debug.Log("SENDING TEAM CHAT!");
        // read text from bar, send it off to the networked chat
        FindObjectOfType<DebriefLogs>().SendTeamChatMessage(TeamChatbar.text, false);
        // Chat window should stay selected when the users send a message
        //  however, pressing enter causes it to deselect the box.
        ClearAndActivateTeamChat();
    }

    public void ClearAndActivateTeamChat()
    {
        Debug.Log("CLEARING AND ACTIVATING TEAM CHAT");
        // EventSystem.SetSelectedGameObject(Chatbar.gameObject);
        TeamChatbar.text = "";
        EventSystem.SetSelectedGameObject(TeamChatbar.gameObject);
        TeamChatbar.ActivateInputField();
    }

    public void DeselectTeamChat()
    {
        EventSystem.SetSelectedGameObject(null);
        TeamChatbar.text = "";
    }

    public void AddTeamMessage(ChatMessage message, bool announcement)
    {
        GameObject newMessage = Instantiate(MessagePrefab);
        TMP_Text messageText = newMessage.GetComponent<TMP_Text>();
        if (announcement)
        {
            messageText.text = $"{message.Message}";
            messageText.color = Color.red;
        }
        else
        {
            messageText.text = $"{message.PlayerName}: {message.Message}";
        }

        // message is not prepared to be scaled and put into box yet, need to wait on content fitter
        ChatMessageUI messageScript = newMessage.GetComponent<ChatMessageUI>();
        messageScript.OnSizeLoaded += AlignTeamMessage;
    }

    public void AlignTeamMessage(float newHeight, GameObject newMessage)
    {
        TMP_Text messageText = newMessage.GetComponent<TMP_Text>();
        RectTransform messageTransform = newMessage.GetComponent<RectTransform>();

        // move to position
        newMessage.transform.SetParent(ChatContentWindow.transform);
        messageTransform.anchoredPosition = new Vector2(MessageX, TeamLastMessageLocation - newHeight / 2);
        messageTransform.localScale = Vector3.one;

        // update CurrentHeight and LastMessageLocation
        TeamCurrentHeight += newHeight;
        TeamLastMessageLocation -= newHeight;

        // set content window to match current height for scrolling
        ChatContentWindow.sizeDelta = new Vector2(ChatContentWindow.sizeDelta.x, TeamCurrentHeight);

        // unsubscribe
        ChatMessageUI messageScript = newMessage.GetComponent<ChatMessageUI>();
        messageScript.OnSizeLoaded -= AlignTeamMessage;
    }

    // method to check if the chat is currently selected
    public bool CheckTeamChatSelected()
    {
        return EventSystem && EventSystem.currentSelectedGameObject == TeamChatbar.gameObject;
    }

    // ------------------------ CARDS ------------------------
    public void FlipCard()
    {
        CardContentBox.text = CurrentCard.Answer;
    }

    public void LoadCardContents(Card card)
    {
        CurrentCard = card;
        CardContentBox.text = CurrentCard.Question;
    }

    public void FlipCardButtonCallback()
    {
        FindObjectOfType<DebriefLogs>(false).FlipCardServerRpc();
    }
}
