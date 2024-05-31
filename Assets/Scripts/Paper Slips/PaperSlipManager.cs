using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

// Network object to control where the paper slips are placed in the scene
public class PaperSlipManager : NetworkBehaviour
{
    public static PaperSlipManager Instance;

    [SerializeField] private List<GameObject> PaperSlips;

    public void AddPaperSlip(GameObject paperSlip)
    {
        PaperSlips.Add(paperSlip);
    }
}
