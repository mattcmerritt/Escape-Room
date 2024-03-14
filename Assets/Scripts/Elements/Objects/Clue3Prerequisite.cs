using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Unity.Netcode;

public class Clue3Prerequisite : Book
{
    [SerializeField] private GameObject Clue3Object;
    private Collider Clue3Collider;
    private MeshRenderer[] Clue3ChildMeshes;
    
    protected override void Start() {
        base.Start();

        // Clue3Collider = Clue3Object.GetComponent<Collider>();
        // Clue3ChildMeshes = Clue3Object.GetComponentsInChildren<MeshRenderer>();

        // Clue3Collider.enabled = false;
        // foreach (MeshRenderer mesh in Clue3ChildMeshes)
        // {
        //     mesh.enabled = false;
        // }
    }

    public override void InteractAllClients(PlayerInteractions player) {
        base.InteractAllClients(player);

        // activate the next clue when this one is collected
        // SequenceManager.Instance.PickUpDSMGuideServerRpc();
    }
}
