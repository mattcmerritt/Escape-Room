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

    // Network variable to sync the collection state
    NetworkVariable<bool> IsCollected = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

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

    public override void OnNetworkSpawn()
    {
        IsCollected.OnValueChanged += (bool previousValue, bool newValue) =>
        {
            // removing the object when it is collected
            if (newValue == true)
            {
                // cannot simply destroy the object, as the behavior script needs to remain in the scene
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
        };
    }

    public void Collect()
    {
        Inventory.AddItem(this);

        // Collect the object for all clients
        CollectServerRpc();
    }

    // This method will be the behavior that will need to be executed for all clients when used
    // Behaviors that should only happen client-side (launching a UI or toggling a state) will still happen in interact
    public virtual void InteractAllClients(PlayerInteractions player)
    {
        // This method does not currently do anything by default
    }

    [ServerRpc(RequireOwnership = false)]
    private void CollectServerRpc()
    {
        IsCollected.Value = true;
    }
}
