using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class Magnet : MonoBehaviour, IDragHandler, IPointerClickHandler, IPointerDownHandler
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
}
