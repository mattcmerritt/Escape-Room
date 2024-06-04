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
    [SerializeField] private ScrollRect ChatScrollWindow;

    private float TeamCurrentHeight = 0, TeamLastMessageLocation = 0;
    private EventSystem EventSystem;
    public static string TeamChatlogObjectName;

    // Phone conversation elements
    [SerializeField] private RectTransform PhoneContentWindow;
    [SerializeField] private ScrollRect PhoneScrollWindow;
    // [SerializeField] private TMP_InputField PhoneChatbar;

    private float PhoneCurrentHeight = 0, PhoneLastMessageLocation = 0;
    // public static string PhoneChatlogObjectName;

    [SerializeField] private Button TakeControlButton, ProceedToDebriefButton;
    [SerializeField] private GameObject InputScrollView;

    private void Start()
    {
        RectTransform prefabMessageTransform = MessagePrefab.GetComponent<RectTransform>();
        MessageX = prefabMessageTransform.anchoredPosition.x;

        EventSystem = FindObjectOfType<EventSystem>();
        TeamChatlogObjectName = TeamChatbar.name;

        // PhoneChatlogObjectName = PhoneChatbar.name;

        TakeControlButton.onClick.AddListener(() =>
        {
            FindObjectOfType<MultipleChoicePhoneCallLogs>().TakePhoneControl();
        });

        ProceedToDebriefButton.onClick.AddListener(() =>
        {
            // go to the debrief
            FindObjectOfType<DebriefPhone>().SwitchUIForAllServerRpc();
        });

        ProceedToDebriefButton.interactable = false;
    }

    // ------------------------ DEBRIEF BUTTON ------------------------
    public void EnableDebriefButton()
    {
        ProceedToDebriefButton.interactable = true;
    }

    // ------------------------ TEAM CONVERSATION ------------------------
    public void SendTeamChatMessage()
    {
        Debug.Log("SENDING TEAM CHAT!");
        // read text from bar, send it off to the networked chat
        FindObjectOfType<MultipleChoicePhoneCallLogs>().SendTeamChatMessage(TeamChatbar.text, false);
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

        // reset content window to bottom to show most recent message
        ChatScrollWindow.verticalNormalizedPosition = 0;
    }

    // method to check if the chat is currently selected
    public bool CheckTeamChatSelected()
    {
        return EventSystem && EventSystem.currentSelectedGameObject == TeamChatbar.gameObject;
    }

    // ------------------------ PHONE CONVERSATION ------------------------
    public void EnablePhone()
    {
        TakeControlButton.interactable = false;
        // TODO: finish implementing, causing errors as is
        FindObjectOfType<MultipleChoicePhoneCallLogs>().GenerateButtonsForCurrentLineServerRpc();
        // PhoneChatbar.interactable = true;
    }
    
    public void DisablePhone()
    {
        TakeControlButton.interactable = false;
        // TODO: finish implementing, causing errors as is
        // FindObjectOfType<MultipleChoicePhoneCallLogs>().ClearButtonsServerRpc(); // TODO: causes buttons to clear with multiple players
        // PhoneChatbar.interactable = false;
    }

    public void ResetPhone()
    {
        TakeControlButton.interactable = true;
        // TODO: finish implementing, causing errors as is
        // PhoneChatbar.interactable = false;
        // FindObjectOfType<MultipleChoicePhoneCallLogs>().ClearButtonsServerRpc(); // TODO: causes buttons to clear with multiple players
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

    public void AddPhoneMessage(ChatMessage message)
    {
        GameObject newMessage = Instantiate(MessagePrefab);
        TMP_Text messageText = newMessage.GetComponent<TMP_Text>();
        messageText.text = $"{message.PlayerName}: {message.Message}";

        // color certain elements
        if(message.PlayerName == "Team")
        {
            messageText.color = Color.white;
        }
        else if(message.PlayerName == "Speaker")
        {
            messageText.color = new Color(0f, 0.25f, 0.5f);
        }
        else if(message.PlayerName == "CONVERSATION FAILED")
        {
            messageText.color = Color.red;
        }

        // message is not prepared to be scaled and put into box yet, need to wait on content fitter
        ChatMessageUI messageScript = newMessage.GetComponent<ChatMessageUI>();
        messageScript.OnSizeLoaded += AlignPhoneMessage;

        // send a system message to explain the rewind if the conversation was failed
        if(message.PlayerName == "CONVERSATION FAILED")
        {
            GameObject resetMessage = Instantiate(MessagePrefab);
            TMP_Text resetText = resetMessage.GetComponent<TMP_Text>();
            resetText.text = "Rewinding conversation to last message...";
            resetText.color = Color.green;
            
            // message is not prepared to be scaled and put into box yet, need to wait on content fitter
            ChatMessageUI resetMessageScript = resetMessage.GetComponent<ChatMessageUI>();
            resetMessageScript.OnSizeLoaded += AlignPhoneMessage;
        }
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

        // reset content window to bottom to show most recent message
        PhoneScrollWindow.verticalNormalizedPosition = 0;
    }

    public GameObject GetInputScrollView()
    {
        return InputScrollView;
    }
}
