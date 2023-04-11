using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Cabinet : SimpleObject
{
    // Cabinet Information and Game Objects
    private Animator Ani;

    // Lock Information and GameObject
    [SerializeField] private bool IsLocked;
    [SerializeField] private bool IsNumeric;
    private List<char> PossibleDigits;
    [SerializeField] private List<char> LockCombination;
    [SerializeField] private List<char> CurrentCombination;
    [SerializeField] private GameObject LockObject;
    [SerializeField] private string LockID;

    // List of all UI objects that need to be updated
    [SerializeField] private List<LockUI> LockUIs;

    // Information that needs to be shared between all clients
    private NetworkVariable<bool> IsOpen = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

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

        // Prepare a list of all the different UI scripts that manage the lock UI
        if (IsLocked)
        {
            LockUIs = new List<LockUI>();
        }
    }

    public override void OnNetworkSpawn()
    {
        // prepare a listener for when the open network variable changes
        IsOpen.OnValueChanged += (bool previousValue, bool newValue) =>
        {
            Debug.Log("Cabinet State changing to: " + (newValue ? "Open" : "Closed"));
            Ani.SetTrigger("Change");
        };
    }

    // Function for when the player clicks on the cabinet
    public override void Interact(PlayerInteractions player)
    {
        // Generating the lock UI and saving a reference
        if (IsLocked)
        {
            // Finding the lock UI that shares the same unique key as this cabinet
            LockUI[] locks = player.GetComponentsInChildren<LockUI>(true);

            Debug.Log(player.name);

            LockUI matchingLock = null;
            foreach (LockUI lockUI in locks)
            {
                Debug.Log(lockUI.GetLockID());
                if (lockUI.GetLockID() == LockID)
                {
                    matchingLock = lockUI;
                }
            }

            // Generating on-click events for the lock digit buttons if not already done
            if (!LockUIs.Contains(matchingLock))
            {
                LockUIs.Add(matchingLock);
                matchingLock.LoadUI(this);
            }
        }

        // If the cabinet has already been unlocked, open or close the doors
        if (!IsLocked)
        {
            ToggleOpenServerRpc();
            return; // do not open up the UI if it is unlocked
        }

        base.Interact(player);
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

        // Update displays for all known UIs
        foreach (LockUI lockUI in LockUIs)
        {
            lockUI.UpdateDigit(index, CurrentCombination[index]);
        }
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

        // Update displays for all known UIs
        foreach (LockUI lockUI in LockUIs)
        {
            lockUI.UpdateDigit(index, CurrentCombination[index]);
        }
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

    [ServerRpc(RequireOwnership = false)]
    private void ToggleOpenServerRpc()
    {
        IsOpen.Value = !IsOpen.Value;
    }
}
