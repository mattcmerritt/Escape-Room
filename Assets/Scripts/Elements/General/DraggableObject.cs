using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Class to handle the interactable and rotatable objects that
// can be viewed in a 3D menu.
// Rotates by clicking and dragging the mouse as opposed to WASD.

// Code based on https://gamedevbeginner.com/how-to-rotate-in-unity-complete-beginners-guide/
public class DraggableObject : SimpleObject
{
    [SerializeField] protected bool IsCopy; // flag used to disable interactions for copied dominos
    [SerializeField] protected DraggableObject Original; // for the copies, allows them to send data back to the main one

    [SerializeField, TextArea(5, 15)] private string ItemInstructions; // the description displayed at the bottom of the panel for how to interact with the 3D object

    [SerializeField] private bool ApplyForce;
    [SerializeField] private float Strength = 100;
    [SerializeField] private float RotationX, RotationY;
    [SerializeField] private Rigidbody Rb;
    [SerializeField] private GameObject ObjectViewer;
    [SerializeField] private RenderTexture SampleRenderTexture;

    protected override void Start() {
        Rb = GetComponent<Rigidbody>();
    }

    protected virtual void Update()
    {
        if (IsCopy)
        {
            if(Input.GetMouseButton(0)) {
                ApplyForce = true;
                RotationX = Input.GetAxis("Mouse X") * Strength;
                RotationY = Input.GetAxis("Mouse Y") * Strength;
            }
            else {
                ApplyForce = false;
            }
        }
    }

    private void FixedUpdate() {
        if(ApplyForce) {
            Rb.AddTorque(RotationY, -RotationX, 0);
        }
    }

    // Create a viewing copy of the object when the player enters the interact menu
    public override void Interact(PlayerInteractions player)
    {
        // if the object is a viewing copy, it cannot be interacted with
        if (IsCopy)
        {
            return;
        }

        // instantiate object viewer
        GameObject viewer = Instantiate(ObjectViewer);
        viewer.name = "Object Viewer";
        viewer.transform.position *= 1; // TODO: determine player number to make sure overlap doesnt occur

        // create render texture for player ui
        RenderTexture rt = new RenderTexture(SampleRenderTexture);
        viewer.GetComponentInChildren<Camera>().targetTexture = rt;

        // copies the object, but disables all scripts so that they are not copied over
        // to the viewing copy
        GameObject copy = Instantiate(gameObject, viewer.transform.GetChild(0)); 
        copy.transform.localPosition = new Vector3(0, 0, 0);
        copy.name = "Viewing Copy: " + copy.name;
        DraggableObject copyScript = copy.GetComponent<DraggableObject>();
        copyScript.SetAsCopy(this);

        player.UpdatePanelInstructions(ItemInstructions, PanelID);

        base.Interact(player);

        // set render texture on player ui
        player.GetComponentInChildren<RawImage>().texture = rt;
    }

    public void SetAsCopy(DraggableObject original)
    {
        IsCopy = true;
        Original = original;
    }
}
