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

    public virtual void Collect()
    {
        StartCoroutine(CollectAfterLoad());
    }

    // important for some objects that are not active before initial usage
    private IEnumerator CollectAfterLoad()
    {
        yield return new WaitUntil(() => Inventory != null);
        Inventory.AddItem(this);

        // show a popup
        UIManager manager = FindObjectOfType<UIManager>();
        manager.ShowPopupPanel(ItemDetails.Name, ItemDetails.Icon);

        // Collect the object for all clients
        CollectServerRpc();
    }

    // Override the interact method to not show a normal UI panel, but instead update the Item panel
    public override void Interact(PlayerInteractions player)
    {
        if (PanelID != "None")
        {
            player.OpenItemWithUIManager(PanelID);
        }
    }

    // This method will be the behavior that will need to be executed for all clients when used
    // Behaviors that should only happen client-side (launching a UI or toggling a state) will still happen in interact
    public virtual void InteractAllClients(PlayerInteractions player)
    {
        // previously all players would open an item menu at the same time
        // Interact(player); // perform interact locally

        // now players do not do anything on their individual machines
        // when items are marked as used, this method is called for the clues
        // this happens in UseItemClientRpc on SharedInventory
    }

    [ServerRpc(RequireOwnership = false)]
    protected virtual void CollectServerRpc()
    {
        IsCollected.Value = true;
    }
}
