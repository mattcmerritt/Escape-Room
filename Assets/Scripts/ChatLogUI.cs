using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class ChatLogUI : MonoBehaviour
{
    [SerializeField] private RectTransform ContentWindow;
    [SerializeField] private GameObject MessagePrefab;
    [SerializeField] private TMP_InputField Chatbar;
    private float CurrentHeight = 0, LastMessageLocation = 0, MessageX;
    private EventSystem EventSystem;

    private void Start()
    {
        RectTransform prefabMessageTransform = MessagePrefab.GetComponent<RectTransform>();
        MessageX = prefabMessageTransform.anchoredPosition.x;

        EventSystem = FindObjectOfType<EventSystem>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (EventSystem.current.currentSelectedGameObject == Chatbar.gameObject)
            {
                Debug.Log("SENDING!");
                // read text from bar, send it off to the networked chat
                FindObjectOfType<TextChat>().SendChatMessage(Chatbar.text);
                EventSystem.SetSelectedGameObject(null);
            }
            else
            {
                Debug.Log("ACTIVATING CHAT!");
                EventSystem.SetSelectedGameObject(Chatbar.gameObject);
            }
        }
    }

    public void AddMessage(ChatMessage message)
    {
        GameObject newMessage = Instantiate(MessagePrefab);
        TMP_Text messageText = newMessage.GetComponent<TMP_Text>();
        messageText.text = $"{message.PlayerName}: {message.Message}";

        // message is not prepared to be scaled and put into box yet, need to wait on content fitter
        ChatMessageUI messageScript = newMessage.GetComponent<ChatMessageUI>();
        messageScript.OnSizeLoaded += AlignMessage;
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
    }
}
