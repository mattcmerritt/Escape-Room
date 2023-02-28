using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Clue Item")]
public class ClueItem : InventoryItem
{
    [TextArea(5, 15)]
    public string Description;
    [TextArea(5, 15)]
    public string Announcement;
}