using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputPanel : MonoBehaviour
{
    [SerializeField] private string Result;

    public void UpdateSolution(string newSolution)
    {
        if (newSolution.ToLower() == Result.ToLower())
        {
            SequenceManager.Instance.PickUpDSMGuideServerRpc();
        }
    }
}
