using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "New Inventory Item")]
public class InventoryItem : ScriptableObject
{
    public string Name;
    public Sprite Icon;
    public Sprite UsedIcon;
    [TextArea(5, 15)]
    public string Description;
    public bool Usable = true; // if the item will have a Use button in the inventory

    // information related to the current escape
    public bool IsStillNecessary;
    public bool HasBeenViewed;

    public void OnEnable()
    {
        IsStillNecessary = true;
        HasBeenViewed = false;
    }
}
