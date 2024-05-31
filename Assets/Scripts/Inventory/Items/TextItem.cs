using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Text Item")]
public class TextItem : InventoryItem
{
    [TextArea(5, 30)]
    public string FullDescription;
}
