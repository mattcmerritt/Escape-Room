using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaperFromBook : UtilityObject
{
    public override void Interact(PlayerInteractions player) 
    {
        UIManager manager = FindObjectOfType<UIManager>();
        manager.OpenUI("Paper");
    }
}
