using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIPanel : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    [SerializeField] private bool Focused;
    [SerializeField] private SimpleObject Object;

    private void Update() 
    {
        if (!Focused && Input.GetMouseButtonDown(0)) 
        {
            Debug.Log("DEACTIVATING WINDOW");
            Object.ExitInteract();
        }
    }

    public void SetActive(bool value)
    {
        gameObject.SetActive(value);
    }

    // Method to associate UI and game element so that they can interact
    public void AttachObject(SimpleObject obj)
    {
        Object = obj;
    }

    // Detects if a click occurs on a UI element
    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log(gameObject.name + " was Pressed");
    }

    // Detects if the mouse is in a UI element
    public void OnPointerEnter(PointerEventData eventData) 
    {
        Debug.Log("Entering " + gameObject.name);
        Focused = true;
    }

    // Detects if the mouse leaves a UI element
    public void OnPointerExit(PointerEventData eventData) 
    {
        Debug.Log("Leaving " + gameObject.name);
        Focused = false;
    }
}
