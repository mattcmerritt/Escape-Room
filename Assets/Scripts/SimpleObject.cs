using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SimpleObject : MonoBehaviour
{
    // UI to display when the object is used
    [SerializeField] private GameObject InteractInterface;
    // Necessary reference to prevent character movement and interaction in the menus
    private PlayerMovement Movement;
    private PlayerInteractions Interactions;

    protected virtual void Start()
    {
        Movement = FindObjectOfType<PlayerMovement>();
        Interactions = FindObjectOfType<PlayerInteractions>();
    }

    protected void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ExitInteract();
        }
    }

    // Function for when the player clicks on or uses the object
    public virtual void Interact()
    {
        InteractInterface.SetActive(true);
        Movement.LockCamera();
        Interactions.OpenMenu();
    }

    // Function for when the player puts away the object
    public virtual void ExitInteract()
    {
        InteractInterface.SetActive(false);
        Movement.UnlockCamera();
        Interactions.CloseMenu();
    }
}
