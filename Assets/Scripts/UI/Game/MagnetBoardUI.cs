using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MagnetBoardUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private GameObject Front, Back;
    [SerializeField] private bool IsFlipped;

    private bool Focused; // whether the mouse is on the UI panel

    // If a click occurs, flip to the other side
    private void Update()
    {
        if (Focused && Input.GetMouseButtonDown(1))
        {
            IsFlipped = !IsFlipped;
            Front.SetActive(!IsFlipped);
            Back.SetActive(IsFlipped);
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
