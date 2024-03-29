using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PillBottle : DraggableObject 
{
    [SerializeField] private Animator Animator;
    private bool IsOpen;

    // Mesh for the label text
    [SerializeField] private Mesh LabelTextMesh;
    [SerializeField] private MeshFilter LabelTextMeshFilter;

    // Information for the slip of paper inside the bottle
    [SerializeField, TextArea(5, 15)] private string Letters, Note;
    [SerializeField] private Sprite PaperIcon;
    [SerializeField] private bool ContainsPaper;

    protected override void Start()
    {
        base.Start();

        // set up the bottle with the associated mesh
        LabelTextMeshFilter.mesh = LabelTextMesh;

        if (IsCopy && ((PillBottle)Original).CheckOpen())
        {
            OpenBottleImmediate();
        }
    }

    protected override void Update()
    {
        base.Update();

        // allow the user to right click on the copy of the object to open it
        if (!IsOpen && IsCopy && Input.GetMouseButtonDown(1))
        {
            OpenBottle();
            ((PillBottle)Original).OpenBottle();

            if (ContainsPaper)
            {
                // find the player's UI manager and display a popup telling them that an item was collected
                UIManager manager = FindObjectOfType<UIManager>();
                manager.ShowPopupPanel("Paper Slip", PaperIcon);
            }
            
        }

        // the copy is not networked, needs to be updated based on the parent which is
        if (IsCopy && ((PillBottle)Original).CheckOpen())
        {
            Animator.SetTrigger("Open");
        }
    }

    public void OpenBottle()
    {
        // Debug.Log("opening " + gameObject.name);
        OpenBottleForAllServerRpc();
    }

    // plays an animation that does not have a duration, just skips to the end frame
    // used for when the player interacts with an open bottle, and it needs to change before
    // they see the closed version
    private void OpenBottleImmediate()
    {
        Animator.SetTrigger("Open Immediate");
        IsOpen = true;
    }

    public bool CheckOpen()
    {
        return IsOpen;
    }

    [ServerRpc(RequireOwnership = false)]
    private void OpenBottleForAllServerRpc()
    {
        OpenBottleClientRpc();
    }

    [ClientRpc]
    private void OpenBottleClientRpc()
    {
        // find the paper slip collection in the scene and add the paper from inside the bottle
        if (!IsCopy && !IsOpen && IsOwner && ContainsPaper)
        {
            PaperSlipCollection papers = FindObjectOfType<PaperSlipCollection>();
            papers.AddPaperToCollection(Letters, Note);
        }

        // Debug.Log("animating " + gameObject.name);
        Animator.SetTrigger("Open");
        IsOpen = true;
    }

    public override void Interact(PlayerInteractions player) {
        base.Interact(player);
        GameObject copy = GameObject.Find("Object Viewer").GetComponentInChildren<PillBottle>().gameObject;
        copy.transform.localScale *= 1.5f; // TODO: get it to not clip into the camera
    }
}
