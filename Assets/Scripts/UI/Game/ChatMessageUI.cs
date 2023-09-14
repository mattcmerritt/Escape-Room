using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Currently, the height of the messages is being managed by a Content Size Fitter component.
// However, this content size fitter will not activate until after start and awake are complete.
// As such, this class will keep checking on the size of the height, and once that is non-zero, it
// will let the chatlog know how large it should be using an event.

public class ChatMessageUI : MonoBehaviour
{
    public event Action<float, GameObject> OnSizeLoaded;
    private RectTransform MessageTransform;

    private void Awake()
    {
        MessageTransform = GetComponent<RectTransform>();
    }

    private void Update()
    {
        if (MessageTransform.sizeDelta.y > 0)
        {
            OnSizeLoaded?.Invoke(MessageTransform.sizeDelta.y, gameObject);
        }
    }
}
