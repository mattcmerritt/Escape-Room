using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Cabinet : SimpleObject
{
    // Cabinet Information and Game Objects
    [SerializeField] private bool IsOpen;
    private Animator Ani;

    // Lock Information and GameObject
    [SerializeField] private bool IsLocked;
    [SerializeField] private bool IsNumeric;
    private List<char> PossibleDigits;
    [SerializeField] private List<char> LockCombination;
    [SerializeField] private List<char> CurrentCombination;
    [SerializeField] private GameObject LockObject;

    // Lock UI Components
    [SerializeField] private List<Button> IncrementButtons, DecrementButtons;
    [SerializeField] private List<TMP_Text> DisplayDigits;
    [SerializeField] private Button InputCombinationButton;

    protected override void Start()
    {
        base.Start(); // still need to grab movement stuff for base class

        Ani = GetComponent<Animator>();

        // Generate a list of all letters the lock can cycle through
        if (!IsNumeric)
        {
            PossibleDigits = new List<char>(26);
            for (int i = 0; i < PossibleDigits.Capacity; i++)
            {
                PossibleDigits.Add((char)((int)'A' + i));
            }
        }
        // Otherwise generate a list of all digits the lock can cycle through
        else
        {
            PossibleDigits = new List<char>(10);
            for (int i = 0; i < PossibleDigits.Capacity; i++)
            {
                PossibleDigits.Add((char)((int)'0' + i));
            }
        }

        if (IsLocked)
        {
            // Generating on-click events for the lock digit buttons
            for (int i = 0; i < IncrementButtons.Count && i < DecrementButtons.Count; i++)
            {
                // need to separate out the memory reseved for the variable on each pass of
                // the loop, otherwise it will copy the final value of i to all instances
                int temp = i;
                IncrementButtons[temp].onClick.AddListener(() => IncrementDigit(temp));
                DecrementButtons[temp].onClick.AddListener(() => DecrementDigit(temp));
            }

            InputCombinationButton.onClick.AddListener(() => AttemptCombination());
        }
    }

    // Function for when the player clicks on the cabinet
    public override void Interact()
    {
        // If the cabinet has already been unlocked, open or close the doors
        if (!IsLocked)
        {
            IsOpen = !IsOpen;
            Debug.Log("Cabinet State changing to: " + (IsOpen ? "Open" : "Closed"));
            Ani.SetTrigger("Change");
            return; // do not open up the UI if it is unlocked
        }

        base.Interact();
    }

    // Function to update single digit of the current code using buttons
    public void IncrementDigit(int index)
    {
        // Get the current character value on the lock
        char current = CurrentCombination[index];
        // Get the index of the current character value
        int curIndex = PossibleDigits.FindIndex((c) => c == current);

        int newIndex = curIndex + 1;
        
        // Force wrapping
        newIndex %= PossibleDigits.Count;
        // Replace letter in combination
        CurrentCombination[index] = PossibleDigits[newIndex];

        // Update display
        DisplayDigits[index].text = "" + CurrentCombination[index];
    }

    // Function to update single digit of the current code using buttons
    public void DecrementDigit(int index)
    {
        // Get the current character value on the lock
        char current = CurrentCombination[index];
        // Get the index of the current character value
        int curIndex = PossibleDigits.FindIndex((c) => c == current);

        // Need to cycle forward one full iteration of the list to avoid modulus
        // with negative values
        int newIndex = curIndex - 1 + PossibleDigits.Count;

        // Force wrapping
        newIndex %= PossibleDigits.Count;
        // Replace letter in combination
        CurrentCombination[index] = PossibleDigits[newIndex];

        // Update display
        DisplayDigits[index].text = "" + CurrentCombination[index];
    }

    public void AttemptCombination()
    {
        bool result = CheckCombination();
        Debug.Log("Code resulted in: " + (result ? "Success" : "Failure"));

        // Removing the lock and updating the room
        if (result)
        {
            IsLocked = false;
            LockObject.SetActive(false);
            // Later we could play a sound here to indicate that the lock is gone.
        }
    }

    private bool CheckCombination()
    {
        // Unlikely to ever trigger, but kept for bounds checking.
        if (LockCombination.Count != CurrentCombination.Count)
        {
            Debug.Log("ERROR: LOCK COMBINATION LENGTHS DO NOT MATCH");
            return false;
        }

        // Go character by character through the code and verify that they match.
        for (int i = 0; i < LockCombination.Count; i++)
        {
            if (LockCombination[i] != CurrentCombination[i])
            {
                return false;
            }
        }

        // Codes match, return true
        return true;
    }
}
