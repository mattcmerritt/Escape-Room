using System.Collections;
using System.Collections.Generic;
using Unity.PlasticSCM.Editor.WebApi;
using UnityEngine;

// Class to handle the interactable and rotatable objects that
// can be viewed in a 3D menu.
// Rotates by clicking and dragging the mouse as opposed to WASD.

// Code based on https://www.youtube.com/watch?v=kplusZYqBok
public class DraggableObject : SimpleObject
{
    [SerializeField] private float CopyRotation; // value used to rotate a copy to face the camera
    [SerializeField] protected bool IsCopy; // flag used to disable interactions for copied dominos
    [SerializeField] protected DraggableObject Original; // for the copies, allows them to send data back to the main one

    [SerializeField, TextArea(5, 15)] private string ItemInstructions; // the description displayed at the bottom of the panel for how to interact with the 3D object

    private Vector3 PrevMousePosition = Vector3.zero;
    private Vector3 MousePositionChange = Vector3.zero;

    protected virtual void Update()
    {
        if (IsCopy)
        {
            if(Input.GetMouseButton(0)) {
                MousePositionChange = Input.mousePosition - PrevMousePosition; // get change to rotate by
                Camera CurrentCamera = FindObjectOfType<Camera>();

                if(Vector3.Dot(transform.up, Vector3.up) >= 0) {
                    transform.RotateAround(transform.position, transform.up, -Vector3.Dot(MousePositionChange, CurrentCamera.transform.right));
                }
                else {
                    transform.RotateAround(transform.position, transform.up, Vector3.Dot(MousePositionChange, CurrentCamera.transform.right));
                }

                transform.RotateAround(transform.position, CurrentCamera.transform.right, Vector3.Dot(MousePositionChange, CurrentCamera.transform.up));
            }
        }

        PrevMousePosition = Input.mousePosition;
    }

    // Create a viewing copy of the object when the player enters the interact menu
    public override void Interact(PlayerInteractions player)
    {
        // if the object is a viewing copy, it cannot be interacted with
        if (IsCopy)
        {
            return;
        }

        player.UpdatePanelInstructions(ItemInstructions, PanelID);

        base.Interact(player);

        // copies the object, but disables all scripts so that they are not copied over
        // to the viewing copy
        GameObject copy = Instantiate(gameObject);
        copy.name = "Viewing Copy: " + copy.name;
        DraggableObject copyScript = copy.GetComponent<DraggableObject>();
        copyScript.SetAsCopy(this);

        // moving the copy to the right place
        Camera camera = player.GetComponentInChildren<Camera>();
        copy.transform.position = camera.gameObject.transform.position + camera.gameObject.transform.forward * 0.25f;
        copy.transform.eulerAngles = copy.transform.eulerAngles + Vector3.right * CopyRotation;
        copy.tag = "Viewing Copy";
    }

    public void SetAsCopy(DraggableObject original)
    {
        IsCopy = true;
        Original = original;
    }
}
