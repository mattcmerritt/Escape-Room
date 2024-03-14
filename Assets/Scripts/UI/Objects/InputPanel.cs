using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputPanel : MonoBehaviour
{
    [SerializeField] private string PanelName;
    [SerializeField] private string Result;
    private string Current;

    public void UpdateSolution(string newSolution)
    {
        Current = newSolution;
    }

    public void CheckSolution()
    {
        if (Current.ToLower() == Result.ToLower())
        {
            SequenceManager.Instance.PickUpDSMGuideServerRpc();
            PlayerInteractions player = FindObjectOfType<PlayerInteractions>(false);
            player.CloseWithUIManager(PanelName);
        }
    }
}
