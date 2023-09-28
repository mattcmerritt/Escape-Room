using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Unity.Netcode;

public class Clue3Prerequisite : Book, IPrerequisiteTrigger
{
    [SerializeField] private GameObject Clue3Object;
    private Collider Clue3Collider;
    private MeshRenderer[] Clue3ChildMeshes;
    
    protected override void Start() {
        base.Start();

        Clue3Collider = Clue3Object.GetComponent<Collider>();
        Clue3ChildMeshes = Clue3Object.GetComponentsInChildren<MeshRenderer>();

        Clue3Collider.enabled = false;
        foreach (MeshRenderer mesh in Clue3ChildMeshes)
        {
            mesh.enabled = false;
        }
    }

    public override void InteractAllClients(PlayerInteractions player) {
        base.InteractAllClients(player);

        TriggerChange();
    }

    public void TriggerChange() {
        TriggerChangeForAllServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    public void TriggerChangeForAllServerRpc() {
        TriggerChangeClientRpc();
    }

    [ClientRpc]
    public void TriggerChangeClientRpc() {
        Debug.Log("Activating Clue 3");

        Clue3Collider.enabled = true;
        foreach (MeshRenderer mesh in Clue3ChildMeshes)
        {
            mesh.enabled = true;
        }
    }
}
