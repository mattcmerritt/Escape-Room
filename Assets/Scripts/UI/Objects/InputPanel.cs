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
        bool correct = false;
        foreach (string option in Results)
        {
            if (Current.ToLower() == option.ToLower())
            {
                correct = true;

                SequenceManager.Instance.FinishDSMGuideServerRpc(PanelName);

                if (IncorrectCoroutine != null)
                {
                    StopCoroutine(IncorrectCoroutine);
                    IncorrectWarning.SetActive(false);
                }

                SharedInventory inventory = FindObjectOfType<SharedInventory>();
                ClueEnvelope Clue3 = GameObject.Find("Envelope 3").GetComponent<ClueEnvelope>();

                if(inventory.CheckForItem(Clue3.ItemDetails.Name) == null)
                {
                    // add approach
                    inventory.AddItem(Clue3);

                    // show a popup
                    UIManager manager = FindObjectOfType<UIManager>();
                    manager.ShowPopupPanel(Clue3.ItemDetails.Name, Clue3.ItemDetails.Icon);
                }
            }
        }  
        // only run if answer matched no cases
        if(!correct)
        {
            if (IncorrectCoroutine != null)
            {
                StopCoroutine(IncorrectCoroutine);
                IncorrectWarning.SetActive(false);
            }
            IncorrectCoroutine = StartCoroutine(ShowIncorrectWarning());
        }
    }

    private IEnumerator ShowIncorrectWarning()
    {
        IncorrectWarning.SetActive(true);
        yield return new WaitForSeconds(3);
        IncorrectWarning.SetActive(false);
    }
}
