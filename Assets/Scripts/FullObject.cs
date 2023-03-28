using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Class to handle the fully interactable objects that
// are viewable in a 3D menu.
public class FullObject : SimpleObject
{
    private GameObject CameraObject;
    private float RotationSpeed = 100f;

    [SerializeField] private float CopyRotation; // value used to rotate a copy to face the camera
    protected bool IsCopy; // flag used to disable interactions for copied dominos
    protected FullObject Original; // for the copies, allows them to send data back to the main one

    protected override void Start()
    {
        base.Start();

        CameraObject = FindObjectOfType<Camera>().gameObject;
    }

    protected virtual void Update()
    {
        float vertical = Input.GetAxis("Vertical");
        float horizontal = Input.GetAxis("Horizontal");

        if (IsCopy)
        {
            transform.localEulerAngles = transform.localEulerAngles + new Vector3(0f, -horizontal, 0f) * Time.deltaTime * RotationSpeed;
        }
    }

    // Create a viewing copy of the object when the player enters the interact menu
    public override void Interact()
    {
        // if the object is a viewing copy, it cannot be interacted with
        if (IsCopy)
        {
            return;
        }

        base.Interact();

        // copies the object, but disables all scripts so that they are not copied over
        // to the viewing copy
        GameObject copy = Instantiate(gameObject);
        copy.name = "Viewing Copy: " + copy.name;
        FullObject copyScript = copy.GetComponent<FullObject>();
        copyScript.SetAsCopy(this);

        // moving the copy to the right place
        copy.transform.position = CameraObject.transform.position + CameraObject.transform.forward * 0.25f;
        copy.transform.eulerAngles = copy.transform.eulerAngles + Vector3.right * CopyRotation;
        copy.tag = "Viewing Copy";
    }

    public void SetAsCopy(FullObject original)
    {
        IsCopy = true;
        Original = original;
    }
}
