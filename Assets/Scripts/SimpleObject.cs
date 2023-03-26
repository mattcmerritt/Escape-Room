using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SimpleObject : MonoBehaviour
{
    private UIManager UIManager; // the active UI manager (will depend on which player this is)
    [SerializeField] private UIPanel InteractUI; // UI to display when the object is used

    protected virtual void Start()
    {
        UIManager = FindObjectOfType<UIManager>();
    }

    // Function for when the player clicks on or uses the object
    public virtual void Interact()
    {
        if (InteractUI != null)
        {
            UIManager.OpenUI(InteractUI);
        }
    }
}
