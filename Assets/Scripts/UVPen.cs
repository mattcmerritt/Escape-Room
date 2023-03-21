using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UVPen : UtilityObject
{
    private bool LightActive;

    // This item will not be usable in the main game world
    public override void Interact()
    {
        LightActive = !LightActive; 
    }

    public bool CheckLight()
    {
        return LightActive;
    }
}
