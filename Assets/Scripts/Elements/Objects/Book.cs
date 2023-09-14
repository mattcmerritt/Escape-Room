using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Book : UtilityObject
{
    [SerializeField] private string URL;
    public override void Interact(PlayerInteractions player)
    {
        Application.OpenURL(URL);
    }
}
