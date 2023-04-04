using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PlayerMovement : NetworkBehaviour
{
    // Basic movement information
    private CharacterController Controller;
    [SerializeField, Range(0, 20)] private float MoveSpeed = 5f;
    [SerializeField, Range(0, 15)] private float Sensitivity = 1.0f;

    // Camera information
    [SerializeField] private GameObject CameraObject;
    private float HorizontalRotation;

    // Camera control
    private bool CameraLocked;

    private void Start()
    {
        // prevent player from spawning in the floor
        if (transform.position.y < 1f)
        {
            transform.position = new Vector3(transform.position.x, 1f, transform.position.z);
        }
    }

    private void Awake() 
    {
        // Start the game with the player's cursor locked
        Cursor.lockState = CursorLockMode.Locked;
        Controller = GetComponent<CharacterController>();
    }

    private void Update()
    {
        // ignore inputs from other players
        if (!IsOwner)
        {
            return;
        }

        // Camera movement controls
        if (!CameraLocked)
        {
            float mouseX = Input.GetAxis("Mouse X") * Sensitivity;
            float mouseY = Input.GetAxis("Mouse Y") * Sensitivity;
            // Rotating the player themselves based on the mouse's horizontal movement
            transform.rotation = Quaternion.Euler(transform.eulerAngles + new Vector3(0f, mouseX, 0f));
            // Rotating the camera inside the player based on the mouse's vertical movement
            HorizontalRotation += -mouseY;
            // Clamping rotation to prevent camera from doing a flip
            HorizontalRotation = Mathf.Clamp(HorizontalRotation, -90f, 90f);
            CameraObject.transform.localRotation = Quaternion.Euler(new Vector3(HorizontalRotation, 0f, 0f));

            // Player movement controls
            float forwardInput = Input.GetAxisRaw("Vertical");
            float sidewaysInput = Input.GetAxisRaw("Horizontal");
            // Normalizing the input vector to keep the player a consistent movement speed
            Vector3 normalizedInput = Vector3.Normalize(new Vector3(sidewaysInput, 0f, forwardInput));
            // Direct the input vectors based on the player's current rotation
            Vector3 movement = (normalizedInput.z * transform.forward) + (normalizedInput.x * transform.right);
            // Applying movement and speed
            Controller.Move(movement * Time.deltaTime * MoveSpeed);
        }        
    }

    public void LockCamera()
    {
        CameraLocked = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void UnlockCamera()
    {
        CameraLocked = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
}
