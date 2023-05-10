using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class TextMagnet : MonoBehaviour, IPointerClickHandler
{
    // Timing variables
    [SerializeField] private float DoubleClickWindow;
    private float LastClick;

    // Blown up image for reading titles
    [SerializeField] private TMP_Text LargeText;
    [SerializeField] private GameObject LargeTextPanel;

    // Contents
    [SerializeField, TextArea(5, 15)] private string Contents;

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

    public void OnPointerClick(PointerEventData eventData)
    {
        // moving the most recent slip to the top
        transform.SetAsLastSibling();

        float clickTime = Time.time;

        // Debug.Log(clickTime - LastClick);

        // if the clicks are close together, show the image blown up
        if (clickTime - LastClick <= DoubleClickWindow)
        {
            LargeTextPanel.SetActive(true);
            LargeText.text = Contents;
        }

        LastClick = clickTime;
    }
}
