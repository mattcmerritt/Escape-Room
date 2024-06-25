using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ScrabbleTile : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler
{
    private RectTransform RectTransform;
    private Vector3 InitialPosition;
    private Vector3 OffsetFromClickPoint;

    private void Start()
    {
        RectTransform = GetComponent<RectTransform>();
        InitialPosition = RectTransform.localPosition;
    }

    // previous code for resetting the tiles - no longer needed
    // public void Update()
    // {
    //     if (Input.GetKeyDown(KeyCode.R))
    //     {
    //         RectTransform.localPosition = InitialPosition;
    //     }
    // }

    public void OnPointerDown(PointerEventData eventData)
    {
        Vector3 globalMousePos;
        RectTransformUtility.ScreenPointToWorldPointInRectangle(transform as RectTransform, eventData.position, eventData.pressEventCamera, out globalMousePos);
        OffsetFromClickPoint = transform.position - globalMousePos;

        transform.position = globalMousePos + OffsetFromClickPoint;
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector3 globalMousePos;
        if (RectTransformUtility.ScreenPointToWorldPointInRectangle(transform as RectTransform, eventData.position, eventData.pressEventCamera, out globalMousePos))
        {
            transform.position = globalMousePos + OffsetFromClickPoint;
        }

        // moving the most recent slip to the top
        transform.SetAsLastSibling();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        RectTransform.localPosition = InitialPosition;
    }
}
