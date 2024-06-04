using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;


public class Magnet : MonoBehaviour, IDragHandler, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler
{
    // Timing variables
    [SerializeField] private float DoubleClickWindow;
    private float LastClick;

    // Blown up image for reading titles
    [SerializeField] private Image LargeImage;
    [SerializeField] private GameObject LargeImagePanel;
    private Vector3 OffsetFromClickPoint;
    private Vector3 OriginalMagnetPosition;
    private SlotText CurrentSlot, PreviousSlot;

    // Id for checking puzzle completion
    [SerializeField] private int MagnetId;

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
        OriginalMagnetPosition = transform.TransformPoint(RectTransformUtility.CalculateRelativeRectTransformBounds(transform).center);
        PreviousSlot = CurrentSlot;
        if(CurrentSlot != null)
        {
            CurrentSlot.SetCurrentMagnet(null);
            CurrentSlot = null;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        // Debug.Log($"Mouse position: {Input.mousePosition}");
        Vector3 magnetCenter = transform.TransformPoint(RectTransformUtility.CalculateRelativeRectTransformBounds(transform).center);

        bool foundSlot = false;
        SlotText[] slots = FindObjectsOfType<SlotText>();
        foreach(SlotText slot in slots)
        {
            Bounds slotBounds = RectTransformUtility.CalculateRelativeRectTransformBounds(slot.gameObject.transform);
            // snap to slot
            // mouse in slot or center of magnet in slot
            if((Input.mousePosition.x <= slot.transform.TransformPoint(slotBounds.max).x && 
               Input.mousePosition.x >= slot.transform.TransformPoint(slotBounds.min).x &&
               Input.mousePosition.y <= slot.transform.TransformPoint(slotBounds.max).y && 
               Input.mousePosition.y >= slot.transform.TransformPoint(slotBounds.min).y) ||
               (magnetCenter.x <= slot.transform.TransformPoint(slotBounds.max).x && 
               magnetCenter.x >= slot.transform.TransformPoint(slotBounds.min).x &&
               magnetCenter.y <= slot.transform.TransformPoint(slotBounds.max).y && 
               magnetCenter.y >= slot.transform.TransformPoint(slotBounds.min).y))
            {
                CurrentSlot = slot;
                // check if something is on this slot already
                if(slot.GetCurrentMagnet() != null)
                {
                    // if the moved magnet was previously on a slot, swap the two
                    if(PreviousSlot != null)
                    {
                        Magnet magnetToSwap = slot.GetCurrentMagnet();
                        magnetToSwap.transform.position = PreviousSlot.transform.TransformPoint(slotBounds.center);
                        magnetToSwap.SetCurrentSlot(PreviousSlot);
                        PreviousSlot.SetCurrentMagnet(magnetToSwap);
                    }
                    // otherwise just move it
                    else
                    {
                        Magnet magnetToSwap = slot.GetCurrentMagnet();
                        magnetToSwap.transform.position = OriginalMagnetPosition;
                        magnetToSwap.SetCurrentSlot(null);
                    }
                }
                slot.SetCurrentMagnet(this);
                transform.position = slot.transform.TransformPoint(slotBounds.center);

                // update slots for all clients using manager
                foundSlot = true;
                MagnetBoardManager.instance.ChangeMagnetSlotServerRpc(MagnetId, slot.GetSlotId());
                MagnetBoardManager.instance.ChangeMagnetPositionServerRpc(MagnetId, transform.position.x, transform.position.y, transform.position.z);
            }
        }
        if (!foundSlot)
        {
            MagnetBoardManager.instance.ChangeMagnetSlotServerRpc(MagnetId, -1);
            MagnetBoardManager.instance.ChangeMagnetPositionServerRpc(MagnetId, transform.position.x, transform.position.y, transform.position.z);
        }
    }

    public void SetCurrentSlot(SlotText s)
    {
        CurrentSlot = s;
    }

    public int GetMagnetId()
    {
        return MagnetId;
    }

    public void MoveMagnetToSlot(int newSlot)
    {
        SlotText[] slots = FindObjectsOfType<SlotText>();
        SlotText slot = Array.Find(slots, (s) => s.GetSlotId() == newSlot);

        if (slot != null)
        {
            Bounds slotBounds = RectTransformUtility.CalculateRelativeRectTransformBounds(slot.gameObject.transform);
            slot.SetCurrentMagnet(this);
            transform.position = slot.transform.TransformPoint(slotBounds.center);
        }
    }
}
