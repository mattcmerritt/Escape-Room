using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputPanel : MonoBehaviour
{
    [SerializeField] private string PanelName;
    [SerializeField] private string[] Results;
    private string Current = "";

    [SerializeField] private GameObject IncorrectWarning;
    private Coroutine IncorrectCoroutine;

    public void UpdateSolution(string newSolution)
    {
        Current = newSolution;

        if (IncorrectCoroutine != null)
        {
            StopCoroutine(IncorrectCoroutine);
            IncorrectWarning.SetActive(false);
        }
    }

    public void CheckSolution()
    {
        foreach (string option in Results)
        {
            if (Current.ToLower() == option.ToLower())
            {
                SequenceManager.Instance.FinishDSMGuideServerRpc(PanelName);

                if (IncorrectCoroutine != null)
                {
                    StopCoroutine(IncorrectCoroutine);
                    IncorrectWarning.SetActive(false);
                }
            }
            else
            {
                if (IncorrectCoroutine != null)
                {
                    StopCoroutine(IncorrectCoroutine);
                    IncorrectWarning.SetActive(false);
                }
                IncorrectCoroutine = StartCoroutine(ShowIncorrectWarning());
            }
        }  
    }

    private IEnumerator ShowIncorrectWarning()
    {
        IncorrectWarning.SetActive(true);
        yield return new WaitForSeconds(3);
        IncorrectWarning.SetActive(false);
    }
}
