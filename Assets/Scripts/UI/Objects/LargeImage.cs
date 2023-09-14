using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class LargeImage : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private GameObject ParentPanel;
    private bool Focused; // whether the mouse is on the UI panel

    // If a click occurs, hide the image preview
    private void Update()
    {
        if (!Focused && Input.GetMouseButtonDown(0))
        {
            ParentPanel.SetActive(false);
        }
    }

    // Detects if the mouse is in a UI element
    public void OnPointerEnter(PointerEventData eventData)
    {
        Focused = true;
    }

    // Detects if the mouse leaves a UI element
    public void OnPointerExit(PointerEventData eventData)
    {
        Focused = false;
    }
}
