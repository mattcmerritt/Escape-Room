using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIPanel : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private UIManager UIManager; // the current player's UI manager
    [SerializeField] private bool Focused; // whether the mouse is on the UI panel

    // If a click occurs, defer to the UIManager to see if panel can be closed
    private void Update() 
    {
        if (!Focused && Input.GetMouseButtonDown(0)) 
        {
            UIManager.CloseUI(this);
        }
    }

    public void Open()
    {
        UIManager.OpenUI(this);
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
