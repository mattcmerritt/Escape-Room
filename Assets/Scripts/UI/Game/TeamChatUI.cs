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

    // Phone conversation elements
    [SerializeField] private RectTransform PhoneContentWindow;
    [SerializeField] private ScrollRect PhoneScrollWindow;

    private float PhoneCurrentHeight = 0, PhoneLastMessageLocation = 0;

    [SerializeField] private Button TakeControlButton, ProceedToDebriefButton;
    [SerializeField] private GameObject InputScrollView;

    private void Start()
    {
        RectTransform prefabMessageTransform = MessagePrefab.GetComponent<RectTransform>();
        MessageX = prefabMessageTransform.anchoredPosition.x;

        // PhoneChatlogObjectName = PhoneChatbar.name;

        TakeControlButton.onClick.AddListener(() =>
        {
            FindObjectOfType<MultipleChoicePhoneCallLogs>().TakePhoneControl(FindObjectOfType<PlayerClientData>().GetPlayerName());
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

    // ------------------------ PHONE CONVERSATION ------------------------
    public void EnablePhone()
    {
        TakeControlButton.interactable = false;
        FindObjectOfType<MultipleChoicePhoneCallLogs>().GenerateButtonsForCurrentLineServerRpc();
    }
    
    public void DisablePhone()
    {
        Debug.Log("local disable");
        TakeControlButton.interactable = false;
    }

    // this is used to create the chat bubble, not the buttons
    // the buttons are handled in MultipleChoicePhoneCallLogs.cs
    public void AddPhoneMessage(ChatMessage message)
    {
        GameObject newMessage = Instantiate(MessagePrefab);
        TMP_Text messageText = newMessage.GetComponent<TMP_Text>();
        if(message.PlayerName != "<SYSTEM MESSAGE - CLEAR>" && message.PlayerName != "<SYSTEM MESSAGE - FAIL>")
        {
            messageText.text = $"{message.PlayerName}: {message.Message}";
        }
        else
        {
            messageText.text = $"{message.Message}";
        }

        // color certain elements
        if(message.PlayerName == "Team")
        {
            messageText.color = Color.white;
        }
        else if(message.PlayerName == "Speaker")
        {
            messageText.color = new Color(0f, 0.25f, 0.5f);
        }
        else if(message.PlayerName == "<SYSTEM MESSAGE - CLEAR>")
        {
            messageText.color = Color.green;
        }
        else if(message.PlayerName == "<SYSTEM MESSAGE - FAIL>")
        {
            messageText.color = Color.red;
        }

        // message is not prepared to be scaled and put into box yet, need to wait on content fitter
        ChatMessageUI messageScript = newMessage.GetComponent<ChatMessageUI>();
        messageScript.OnSizeLoaded += AlignPhoneMessage;

        // send a system message to explain the rewind if the conversation was failed
        if(message.PlayerName == "<SYSTEM MESSAGE - FAIL>")
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
