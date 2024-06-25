using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PillBoxManager : MonoBehaviour
{
    public void OpenPillBoxSlot(string name)
    {
        PillBox[] all = FindObjectsOfType<PillBox>();
        PillBox correctBox = null;
        foreach (PillBox p in all)
        {
            if(p.CheckIfCorrect())
            {
                correctBox = p;
            }
        }

        if(correctBox != null)
        {
            correctBox.OpenPillBoxSlotServerRpc(name);
        }
        else
        {
            Debug.LogError("Couldn't find the original pillbox for data!");
        }
        
    }

    public void OpenSpecialPillBoxSlot()
    {
        PillBox p = FindObjectOfType<PillBox>();
        p.OpenSpecialPillBoxSlot();
    }
}
