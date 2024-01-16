using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class TeamChatUI : MonoBehaviour
{
    [SerializeField] private GameObject MessagePrefab;
    private float MessageX;

    // Team chat elements (functions similarly to the text chat)
    [SerializeField] private RectTransform ChatContentWindow;
    [SerializeField] private TMP_InputField TeamChatbar;

    private float TeamCurrentHeight = 0, TeamLastMessageLocation = 0;
    private EventSystem EventSystem;
    public static string TeamChatlogObjectName;

    // Phone conversation elements
    [SerializeField] private RectTransform PhoneContentWindow;
    [SerializeField] private TMP_InputField PhoneChatbar;

    private float PhoneCurrentHeight = 0, PhoneLastMessageLocation = 0;
    public static string PhoneChatlogObjectName;

    [SerializeField] private Button TakeControlButton;

    private void Start()
    {
        RectTransform prefabMessageTransform = MessagePrefab.GetComponent<RectTransform>();
        MessageX = prefabMessageTransform.anchoredPosition.x;

        EventSystem = FindObjectOfType<EventSystem>();
        TeamChatlogObjectName = TeamChatbar.name;

        PhoneChatlogObjectName = PhoneChatbar.name;

        TakeControlButton.onClick.AddListener(() =>
        {
            FindObjectOfType<PhoneCallLogs>().TakePhoneControl();
        });
    }

    // ------------------------ TEAM CONVERSATION ------------------------
    public void SendTeamChatMessage()
    {
        Debug.Log("SENDING TEAM CHAT!");
        // read text from bar, send it off to the networked chat
        FindObjectOfType<PhoneCallLogs>().SendTeamChatMessage(TeamChatbar.text, false);
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
        return EventSystem.currentSelectedGameObject == TeamChatbar.gameObject;
    }

    // ------------------------ PHONE CONVERSATION ------------------------
    public void EnablePhone()
    {
        TakeControlButton.interactable = false;
        PhoneChatbar.interactable = true;
    }
    
    public void DisablePhone()
    {
        TakeControlButton.interactable = false;
        PhoneChatbar.interactable = false;
    }

    public void ResetPhone()
    {
        TakeControlButton.interactable = true;
        PhoneChatbar.interactable = false;

        TakeControlButton.GetComponentInChildren<TMP_Text>().text = "Restart as Speaker";
    }

    public void ClearConversation()
    {
        // destroy all old messages
        GameObject[] children = new GameObject[PhoneContentWindow.childCount];
        for (int i = 0; i < PhoneContentWindow.childCount; i++)
        {
            children[i] = PhoneContentWindow.GetChild(i).gameObject;
        }

        foreach (GameObject child in children)
        {
            Destroy(child);
        }

        // restart content window
        PhoneCurrentHeight = 0;
        PhoneLastMessageLocation = 0;

        // resize chat
        PhoneContentWindow.sizeDelta = Vector2.zero;
    }

    public void SendPhoneChatMessage()
    {
        Debug.Log("SENDING!");
        // read text from bar, send it off to the networked chat
        FindObjectOfType<PhoneCallLogs>().SendPhoneChatMessage(PhoneChatbar.text);
        // Chat window should stay selected when the users send a message
        //  however, pressing enter causes it to deselect the box.
        ClearAndActivatePhoneChat();
    }

    public void ClearAndActivatePhoneChat()
    {
        Debug.Log("CLEARING AND ACTIVATING");
        // EventSystem.SetSelectedGameObject(Chatbar.gameObject);
        PhoneChatbar.text = "";
        EventSystem.SetSelectedGameObject(PhoneChatbar.gameObject);
        PhoneChatbar.ActivateInputField();
    }

    public void DeselectPhoneChat()
    {
        EventSystem.SetSelectedGameObject(null);
        PhoneChatbar.text = "";
    }

    public void AddPhoneMessage(ChatMessage message)
    {
        GameObject newMessage = Instantiate(MessagePrefab);
        TMP_Text messageText = newMessage.GetComponent<TMP_Text>();
        messageText.text = $"{message.PlayerName}: {message.Message}";

        // message is not prepared to be scaled and put into box yet, need to wait on content fitter
        ChatMessageUI messageScript = newMessage.GetComponent<ChatMessageUI>();
        messageScript.OnSizeLoaded += AlignPhoneMessage;
    }

    public void AlignPhoneMessage(float newHeight, GameObject newMessage)
    {
        TMP_Text messageText = newMessage.GetComponent<TMP_Text>();
        RectTransform messageTransform = newMessage.GetComponent<RectTransform>();

        // move to position
        newMessage.transform.SetParent(PhoneContentWindow.transform);
        messageTransform.anchoredPosition = new Vector2(MessageX, PhoneLastMessageLocation - newHeight / 2);
        messageTransform.localScale = Vector3.one;

        // update CurrentHeight and LastMessageLocation
        PhoneCurrentHeight += newHeight;
        PhoneLastMessageLocation -= newHeight;

        // set content window to match current height for scrolling
        PhoneContentWindow.sizeDelta = new Vector2(PhoneContentWindow.sizeDelta.x, PhoneCurrentHeight);

        // unsubscribe
        ChatMessageUI messageScript = newMessage.GetComponent<ChatMessageUI>();
        messageScript.OnSizeLoaded -= AlignPhoneMessage;
    }

    // method to check if the chat is currently selected
    public bool CheckPhoneChatSelected()
    {
        return EventSystem.currentSelectedGameObject == PhoneChatbar.gameObject;
    }
}
