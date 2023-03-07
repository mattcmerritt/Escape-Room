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

    // Inventory data
    [SerializeField] private SharedInventory Inventory;
    private List<KeyCode> InventoryKeys = new List<KeyCode>{ 
        KeyCode.Alpha1, 
        KeyCode.Alpha2, 
        KeyCode.Alpha3, 
        KeyCode.Alpha4, 
        KeyCode.Alpha5, 
        KeyCode.Alpha6, 
        KeyCode.Alpha7,
        KeyCode.Alpha8,
        KeyCode.Alpha9
    };

    private void Start()
    {
        Inventory = FindObjectOfType<SharedInventory>();
    }

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
                    Debug.Log(hit.collider.gameObject.name);
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

            // Using inventory items
            for (int i = 0; i < InventoryKeys.Count; i++)
            {
                if (Input.GetKeyDown(InventoryKeys[i]))
                {
                    Inventory.UseItem(i);
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
