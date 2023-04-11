using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

// Basic class to represent an object that is collected and added into the
// shared inventory on collection. Once in the inventory, they behave similar
// to SimpleObjects in terms of interactions pulling up a usage menu.
public abstract class UtilityObject : SimpleObject
{
    // Physical object information
    private MeshRenderer MeshRenderer;
    private MeshRenderer[] ChildMeshes;
    private Collider Collider;

    // Item information
    [SerializeField] public InventoryItem ItemDetails;
    protected SharedInventory Inventory;
    [SerializeField] public bool Used;

    protected override void Start()
    {
        base.Start();

        Inventory = FindObjectOfType<SharedInventory>();
        MeshRenderer = GetComponent<MeshRenderer>();
        Collider = GetComponent<Collider>();

        // sometimes, the mesh renderer is not attached to a single object
        // and multiple child objects have mesh renderers that need to be disabled
        ChildMeshes = GetComponentsInChildren<MeshRenderer>();
    }

    public void Collect()
    {
        Inventory.AddItem(this);
        // cannot simply disable the object, as the script to control the UI
        // will stop working and listening for keys
        if (MeshRenderer != null)
        {
            MeshRenderer.enabled = false;
        }
        else if (ChildMeshes != null)
        {
            foreach (MeshRenderer mesh in ChildMeshes)
            {
                mesh.enabled = false;
            }
        }
        Collider.enabled = false;
    }
}
