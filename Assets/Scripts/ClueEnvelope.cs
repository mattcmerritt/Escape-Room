using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClueEnvelope : UtilityObject
{
    public override void UseObject()
    {
        Debug.Log("Used " + ItemDetails.Name);
    }
}
