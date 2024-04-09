using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class SlotText : MonoBehaviour, IPointerClickHandler
{
    // Timing variables
    [SerializeField] private float DoubleClickWindow;
    private float LastClick;

    // Blown up image for reading titles
    [SerializeField] private TMP_Text LargeText;
    [SerializeField] private GameObject LargeTextPanel;
    [SerializeField] private Magnet CurrentMagnet;

    // Contents
    [SerializeField, TextArea(5, 15)] private string Contents;

    // Id for checking puzzle completion
    [SerializeField] private int SlotId;

    // public void OnEnable()
    // {
    //     Debug.Log($"Mouse position: {Input.mousePosition}");
    //     Debug.Log($"Slot position: {RectTransformUtility.CalculateRelativeRectTransformBounds(transform)}");
    // }

    public void OnPointerClick(PointerEventData eventData)
    {
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

    public Magnet GetCurrentMagnet()
    {
        return CurrentMagnet;
    }

    public void SetCurrentMagnet(Magnet m)
    {
        CurrentMagnet = m;
    }

    public int GetSlotId()
    {
        return SlotId;
    }
}

