using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteractions : MonoBehaviour
{
    // Interaction constants
    [SerializeField, Range(0, 25)] private float InteractRange;

    // Camera information
    [SerializeField] private GameObject CameraObject;

    // Inventory data
    [SerializeField] private SharedInventory Inventory;

    private void Start()
    {
        Inventory = FindObjectOfType<SharedInventory>();
    }

    private void Update()
    {
        // Raycast forward to find object in front of the player
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, CameraObject.transform.forward, out hit, InteractRange))
            {
                Debug.Log(hit.collider.gameObject.name);
                UtilityObject util = hit.collider.GetComponent<UtilityObject>();
                if (util != null)
                {
                    util.Collect();
                }
            }
        }
    }
}
