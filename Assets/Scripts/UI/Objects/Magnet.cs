using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class Magnet : MonoBehaviour, IDragHandler, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler
{
    // Timing variables
    [SerializeField] private float DoubleClickWindow;
    private float LastClick;

    // Blown up image for reading titles
    [SerializeField] private Image LargeImage;
    [SerializeField] private GameObject LargeImagePanel;
    private Vector3 OffsetFromClickPoint;

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

    public void OnPointerClick(PointerEventData eventData)
    {
        // moving the most recent slip to the top
        transform.SetAsLastSibling();

        float clickTime = Time.time;

        // Debug.Log(clickTime - LastClick);

        // if the clicks are close together, show the image blown up
        if (clickTime - LastClick <= DoubleClickWindow)
        {
            LargeImagePanel.SetActive(true);
            LargeImage.sprite = GetComponentInChildren<Image>().sprite;

        }

        LastClick = clickTime;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Vector3 mousePosInRect;
        RectTransformUtility.ScreenPointToWorldPointInRectangle(transform as RectTransform, eventData.position, eventData.pressEventCamera, out mousePosInRect);
        OffsetFromClickPoint = transform.position - mousePosInRect;
        // Debug.Log("offset set: " + OffsetFromClickPoint);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        // Debug.Log($"Mouse position: {Input.mousePosition}");
        Vector3 magnetCenter = transform.TransformPoint(RectTransformUtility.CalculateRelativeRectTransformBounds(transform).center);

        SlotText[] slots = FindObjectsOfType<SlotText>();
        foreach(SlotText slot in slots)
        {
            Bounds slotBounds = RectTransformUtility.CalculateRelativeRectTransformBounds(slot.gameObject.transform);
            if(Input.mousePosition.x <= slot.transform.TransformPoint(slotBounds.max).x && 
               Input.mousePosition.x >= slot.transform.TransformPoint(slotBounds.min).x &&
               Input.mousePosition.y <= slot.transform.TransformPoint(slotBounds.max).y && 
               Input.mousePosition.y >= slot.transform.TransformPoint(slotBounds.min).y)
            {
                transform.position = slot.transform.TransformPoint(slotBounds.center);
            }
            else if(magnetCenter.x <= slot.transform.TransformPoint(slotBounds.max).x && 
                    magnetCenter.x >= slot.transform.TransformPoint(slotBounds.min).x &&
                    magnetCenter.y <= slot.transform.TransformPoint(slotBounds.max).y && 
                    magnetCenter.y >= slot.transform.TransformPoint(slotBounds.min).y)
            {
                transform.position = slot.transform.TransformPoint(slotBounds.center);
            }
        }
    }
}
