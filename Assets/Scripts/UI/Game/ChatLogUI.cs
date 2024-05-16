using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.UI;

public class ChatLogUI : MonoBehaviour
{
    [SerializeField] private RectTransform ContentWindow;
    [SerializeField] private GameObject MessagePrefab;
    [SerializeField] private TMP_InputField Chatbar;
    [SerializeField] private GameObject MissedMessageIndicator;
    [SerializeField] private ScrollRect ScrollWindow;
    private bool Closed;
    private float CurrentHeight = 0, LastMessageLocation = 0, MessageX;
    private EventSystem EventSystem;
    public static string ChatlogObjectName;

    private void Start()
    {
        RectTransform prefabMessageTransform = MessagePrefab.GetComponent<RectTransform>();
        MessageX = prefabMessageTransform.anchoredPosition.x;

        EventSystem = FindObjectOfType<EventSystem>();
        ChatlogObjectName = Chatbar.name;
    }

    public void OpenChat()
    {
        Debug.Log("ACTIVATING CHAT!");
        MissedMessageIndicator.SetActive(false);
        Closed = false;
        EventSystem.SetSelectedGameObject(Chatbar.gameObject);
    }

    public void SendChatMessage()
    {
        Debug.Log("SENDING!");
        // read text from bar, send it off to the networked chat
        FindObjectOfType<TextChat>().SendChatMessage(Chatbar.text);
        // Chat window should stay selected when the users send a message
        //  however, pressing enter causes it to deselect the box.
        ClearAndActivateChat();
    }

    public void ClearAndActivateChat()
    {
        Debug.Log("CLEARING AND ACTIVATING");
        // EventSystem.SetSelectedGameObject(Chatbar.gameObject);
        Chatbar.text = "";
        EventSystem.SetSelectedGameObject(Chatbar.gameObject);
        Chatbar.ActivateInputField();
    }

    public void DeselectChat()
    {
        EventSystem.SetSelectedGameObject(null);
        Chatbar.text = "";
        Closed = true;
    }

    public void AddMessage(ChatMessage message)
    {
        GameObject newMessage = Instantiate(MessagePrefab);
        TMP_Text messageText = newMessage.GetComponent<TMP_Text>();
        messageText.text = $"{message.PlayerName}: {message.Message}";

        // message is not prepared to be scaled and put into box yet, need to wait on content fitter
        ChatMessageUI messageScript = newMessage.GetComponent<ChatMessageUI>();
        messageScript.OnSizeLoaded += AlignMessage;

        if (Closed)
        {
            MissedMessageIndicator.SetActive(true);
        }
    }

    public void AlignMessage(float newHeight, GameObject newMessage)
    {
        TMP_Text messageText = newMessage.GetComponent<TMP_Text>();
        RectTransform messageTransform = newMessage.GetComponent<RectTransform>();

        // move to position
        newMessage.transform.SetParent(ContentWindow.transform);
        messageTransform.anchoredPosition = new Vector2(MessageX, LastMessageLocation - newHeight / 2);
        messageTransform.localScale = Vector3.one;

        // update CurrentHeight and LastMessageLocation
        CurrentHeight += newHeight;
        LastMessageLocation -= newHeight;

        // set content window to match current height for scrolling
        ContentWindow.sizeDelta = new Vector2(ContentWindow.sizeDelta.x, CurrentHeight);

        // unsubscribe
        ChatMessageUI messageScript = newMessage.GetComponent<ChatMessageUI>();
        messageScript.OnSizeLoaded -= AlignMessage;

        // reset content window to bottom to show most recent message
        ScrollWindow.verticalNormalizedPosition = 0;
    }

    // method to check if the chat is currently selected
    public bool CheckChatSelected()
    {
        return EventSystem.currentSelectedGameObject == Chatbar.gameObject;
    }
}
