using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FakeBook : DraggableObject
{
    [SerializeField] private MeshRenderer KeyHover;
    private bool FirstTimeSetup;

    protected override void Start()
    {
        base.Start();

        FirstTimeSetup = true;
    }

    protected override void Update() {
        base.Update();
        if(FirstTimeSetup && IsCopy) {
            KeyHover.enabled = true;
            FirstTimeSetup = false;
        }
    }
}
