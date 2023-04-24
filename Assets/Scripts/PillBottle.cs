using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PillBottle : FullObject 
{
    [SerializeField] private Animator Animator;
    private bool IsOpen;

    // Mesh for the label text
    [SerializeField] private Mesh LabelTextMesh;
    [SerializeField] private MeshFilter LabelTextMeshFilter;

    protected override void Start()
    {
        base.Start();

        // set up the bottle with the associated mesh
        LabelTextMeshFilter.mesh = LabelTextMesh;
    }

    protected override void Update()
    {
        base.Update();

        // allow the user to right click on the copy of the object to open it
        if (IsCopy && Input.GetMouseButtonDown(1))
        {
            OpenBottle();
            ((PillBottle)Original).OpenBottle();
        }

        if (IsCopy && ((PillBottle)Original).CheckOpen())
        {
            OpenBottleImmediate();
        }
    }

    public void OpenBottle()
    {
        Animator.SetTrigger("Open");
        IsOpen = true;
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
}
