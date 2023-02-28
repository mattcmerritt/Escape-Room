using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "New Inventory Item")]
public class InventoryItem : ScriptableObject
{
    public string Name;
    public Sprite Icon;
    public Sprite UsedIcon;
}
