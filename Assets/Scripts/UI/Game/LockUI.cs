using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

// Helper script to handle building the lock UI

public class LockUI : MonoBehaviour
{
    // Lock UI Components
    [SerializeField] private List<Button> IncrementButtons, DecrementButtons;
    [SerializeField] private List<TMP_Text> DisplayDigits;
    [SerializeField] private Button InputCombinationButton;

    [SerializeField] private GameObject IncorrectWarning;
    private Coroutine IncorrectCoroutine;

    // ID for UI
    [SerializeField] private string LockID;

    public void LoadUI(Cabinet cabinet)
    {
        // Generating on-click events for the lock digit buttons
        for (int i = 0; i < IncrementButtons.Count && i < DecrementButtons.Count; i++)
        {
            // need to separate out the memory reseved for the variable on each pass of
            // the loop, otherwise it will copy the final value of i to all instances
            int temp = i;
            IncrementButtons[temp].onClick.AddListener(() => cabinet.IncrementDigit(temp));
            DecrementButtons[temp].onClick.AddListener(() => cabinet.DecrementDigit(temp));
        }

        InputCombinationButton.onClick.AddListener(() => {
            bool result = cabinet.AttemptCombination();

            // hide prev warning
            if (IncorrectCoroutine != null)
            {
                StopCoroutine(IncorrectCoroutine);
                IncorrectWarning.SetActive(false);
            }

            if (!result)
            {
                IncorrectCoroutine = StartCoroutine(ShowIncorrectWarning());
            }
        });
    }

    public void UpdateDigit(int index, char value)
    {
        DisplayDigits[index].text = "" + value;
    }

    public string GetLockID()
    {
        return LockID;
    }

    private IEnumerator ShowIncorrectWarning()
    {
        IncorrectWarning.SetActive(true);
        yield return new WaitForSeconds(3);
        IncorrectWarning.SetActive(false);
    }
}
