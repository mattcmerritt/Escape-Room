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

    // Permanently enabled UI objects that should disappear when another menu comes up
    [SerializeField] private GameObject InventoryUI, CrosshairUI, AnnoucementUI;

    protected virtual void Start()
    {
        Movement = FindObjectOfType<PlayerMovement>();
        Interactions = FindObjectOfType<PlayerInteractions>();
    }

    // All menus can be closed with escape, regardless of the type of object
    protected virtual void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ExitInteract();
        }
    }

    // Function for when the player clicks on or uses the object
    public virtual void Interact()
    {
        InventoryUI.SetActive(false);
        CrosshairUI.SetActive(false);
        AnnoucementUI.SetActive(false);

        InteractInterface.SetActive(true);
        Movement.LockCamera();
        Interactions.OpenMenu();
    }

    // Function for when the player puts away the object
    public virtual void ExitInteract()
    {
        InventoryUI.SetActive(true);
        CrosshairUI.SetActive(true);
        AnnoucementUI.SetActive(true);

        Debug.Log(gameObject.name + ": Exiting Interaction");
        InteractInterface.SetActive(false);
        Movement.UnlockCamera();
        Interactions.CloseMenu();
    }
}
