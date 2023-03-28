using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PillBottle : FullObject 
{
    [SerializeField] private Animator Animator;

    protected override void Update()
    {
        base.Update();

        // allow the user to right click on the copy of the object to open it
        if (IsCopy && Input.GetMouseButtonDown(1))
        {
            OpenBottle();
            ((PillBottle)Original).OpenBottle();
        }
    }

    public void OpenBottle()
    {
        Animator.SetTrigger("Open");
    }
}
