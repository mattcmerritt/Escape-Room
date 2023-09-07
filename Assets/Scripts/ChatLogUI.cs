using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ChatLogUI : MonoBehaviour
{
    [SerializeField] private RectTransform ContentWindow;
    [SerializeField] private GameObject MessagePrefab;
    [SerializeField] private float CurrentHeight = 0, LastMessageLocation = 0, MessageX;

    private void Start()
    {
        RectTransform prefabMessageTransform = MessagePrefab.GetComponent<RectTransform>();
        MessageX = prefabMessageTransform.anchoredPosition.x;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            AddMessage();
        }
    }

    private void AddMessage()
    {
        GameObject newMessage = Instantiate(MessagePrefab);
        TMP_Text messageText = newMessage.GetComponent<TMP_Text>();

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

        // unsubscribe
        ChatMessageUI messageScript = newMessage.GetComponent<ChatMessageUI>();
        messageScript.OnSizeLoaded -= AlignMessage;
    }
}
