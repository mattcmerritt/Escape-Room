using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Class to handle the fully interactable objects that
// are viewable in a 3D menu.
public class FullObject : SimpleObject
{
    [SerializeField] private GameObject PhysicalObject;
    private GameObject Copy;
    private GameObject CameraObject;
    private float RotationSpeed = 100f;

    protected override void Start()
    {
        base.Start();

        // grab a reference to the original object to copy
        PhysicalObject = gameObject;
        CameraObject = FindObjectOfType<Camera>().gameObject;
    }

    protected override void Update()
    {
        base.Update();

        float vertical = Input.GetAxis("Vertical");
        float horizontal = Input.GetAxis("Horizontal");

        if (Copy != null)
        {
            Copy.transform.localEulerAngles = Copy.transform.localEulerAngles + new Vector3(0f, 0f, -horizontal) * Time.deltaTime * RotationSpeed;
        }
    }

    // Create a viewing copy of the object when the player enters the interact menu
    public override void Interact()
    {
        base.Interact();

        // copies the object, but disables all scripts so that they are not copied over
        // to the viewing copy
        SimpleObject ownInteractions = GetComponent<SimpleObject>();
        ownInteractions.enabled = false;
        Copy = Instantiate(PhysicalObject);
        ownInteractions.enabled = true;

        Copy.transform.position = CameraObject.transform.position + CameraObject.transform.forward * 0.25f;
        Copy.transform.eulerAngles = Copy.transform.eulerAngles + Vector3.right * -90f;
    }

    // Dispose of viewing copy when the player leaves the interact menu
    public override void ExitInteract()
    {
        base.ExitInteract();

        if (Copy != null)
        {
            Destroy(Copy);
            Copy = null;
        }
    }
}
