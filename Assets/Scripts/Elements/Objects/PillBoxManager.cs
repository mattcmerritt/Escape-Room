using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PillBoxManager : MonoBehaviour
{
    public void OpenPillBoxSlot(string name)
    {
        PillBox.OpenPillBoxSlot(name);
    }

    public void OpenSpecialPillBoxSlot()
    {
        PillBox p = FindObjectOfType<PillBox>();
        p.OpenSpecialPillBoxSlot();
    }
}
