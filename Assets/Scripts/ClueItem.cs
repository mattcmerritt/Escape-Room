using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Clue Item")]
public class ClueItem : InventoryItem
{
    [TextArea(5, 30)]
    public string ClueDescription;
    [TextArea(5, 15)]
    public string Announcement;
}