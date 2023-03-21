using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteractions : MonoBehaviour
{
    // Interaction constants
    [SerializeField, Range(0, 25)] private float InteractRange;

    // Camera information
    [SerializeField] private GameObject CameraObject;

    // Player state
    private bool InMenu;

    private void Update()
    {
        if (!InMenu)
        {
            // Raycast forward to find object in front of the player
            if (Input.GetMouseButtonDown(0))
            {
                RaycastHit hit;
                if (Physics.Raycast(transform.position, CameraObject.transform.forward, out hit, InteractRange))
                {
                    // Debug.Log(hit.collider.gameObject.name);
                    // Checking for components on clicked object to determine function
                    UtilityObject util = hit.collider.GetComponent<UtilityObject>();
                    SimpleObject simple = hit.collider.GetComponent<SimpleObject>();
                    if (util != null)
                    {
                        util.Collect();
                    }
                    else if (simple != null)
                    {
                        simple.Interact();
                    } 
                }
            }
        }
    }

    // Function to prevent player from double interacting with objects if menu is open
    public void OpenMenu()
    {
        InMenu = true;
    }

    // Function to restore player interactions when menu is closed
    public void CloseMenu()
    {
        InMenu = false;
    }
}
