using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputPanel : MonoBehaviour
{
    [SerializeField] private string PanelName;
    [SerializeField] private string[] Results;
    private string Current;

    public void UpdateSolution(string newSolution)
    {
        Current = newSolution;
    }

    public void CheckSolution()
    {
        foreach (string option in Results)
        {
            if (Current.ToLower() == option.ToLower())
            {
                SequenceManager.Instance.PickUpDSMGuideServerRpc(PanelName);
            }
        }  
    }
}
