using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ScrabbleTile : MonoBehaviour, IDragHandler
{
    private RectTransform RectTransform;
    private Vector3 InitialPosition;

    private void Start()
    {
        RectTransform = GetComponent<RectTransform>();
        InitialPosition = RectTransform.localPosition;
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            RectTransform.localPosition = InitialPosition;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector3 globalMousePos;
        if (RectTransformUtility.ScreenPointToWorldPointInRectangle(transform as RectTransform, eventData.position, eventData.pressEventCamera, out globalMousePos))
        {
            transform.position = globalMousePos;
        }

        // moving the most recent slip to the top
        transform.SetAsLastSibling();
    }
}
