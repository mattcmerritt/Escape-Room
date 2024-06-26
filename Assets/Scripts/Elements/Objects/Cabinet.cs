using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Cabinet : SimpleObject
{
    // Struct that will contain the current data for the combination
    public struct Combination : INetworkSerializable
    {
        public char[] Values;

        public Combination(char[] currentValues)
        {
            Values = currentValues;
        }

        void INetworkSerializable.NetworkSerialize<T>(BufferSerializer<T> serializer)
        {
            serializer.SerializeValue(ref Values);
        }
    }

    // Cabinet Information and Game Objects
    private Animator Ani;

    // Lock Information and GameObject
    [SerializeField] private bool StartLocked;
    [SerializeField] private bool IsNumeric;
    private List<char> PossibleDigits;
    [SerializeField] private List<char> LockCombination;

    [SerializeField] private GameObject LockObject;
    [SerializeField] private string LockID;

    // List of all UI objects that need to be updated
    [SerializeField] private List<LockUI> LockUIs;

    // Information that needs to be shared between all clients
    private NetworkVariable<bool> IsOpen = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    private NetworkVariable<Combination> CurrentCombination = new NetworkVariable<Combination>(default, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    private NetworkVariable<bool> IsLocked = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    // Progression and sequencing information
    [SerializeField] private bool IsBlockingProgression;
    [SerializeField] private int NextClueNumber;

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
        if (StartLocked)
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

        // host prepares the starting combination for the lock
        if (IsOwner)
        {
            char[] startValues = new char[LockCombination.Count];
            for (int i = 0; i < startValues.Length; i++)
            {
                startValues[i] = IsNumeric ? '0' : 'A';
            }
            Combination startCombination = new Combination(startValues);

            CurrentCombination.Value = startCombination;
        }
        
        // listener for the list of current values on the lock
        CurrentCombination.OnValueChanged += (Combination previousValue, Combination newValue) =>
        {
            for (int i = 0; i < newValue.Values.Length; i++)
            {
                // Update digit displays for all known UIs
                foreach (LockUI lockUI in LockUIs)
                {
                    lockUI.UpdateDigit(i, CurrentCombination.Value.Values[i]);
                }
            }
        };

        // host prepares the is locked network variable
        if (IsOwner && StartLocked)
        {
            IsLocked.Value = true;
        }

        // listener for when the lock gets unlocked
        IsLocked.OnValueChanged += (bool previousValue, bool newValue) =>
        {
            LockObject.GetComponent<BoxCollider>().enabled = false;
            // Later we could play a sound here to indicate that the lock is gone.
            Ani.SetTrigger("Unlock");

            // Hiding the lock UI for the current player
            PlayerInteractions player = null;
            foreach (PlayerInteractions potentialPlayer in FindObjectsOfType<PlayerInteractions>())
            {
                if (potentialPlayer.IsOwner) player = potentialPlayer;
            }
            player.CloseWithUIManager(PanelID);
        };
    }

    // Function for when the player clicks on the cabinet
    public override void Interact(PlayerInteractions player)
    {
        // Generating the lock UI and saving a reference
        if (IsLocked.Value)
        {
            // Finding the lock UI that shares the same unique key as this cabinet
            LockUI[] locks = player.GetComponentsInChildren<LockUI>(true);

            // Debug.Log(player.name);

            LockUI matchingLock = null;
            foreach (LockUI lockUI in locks)
            {
                // Debug.Log(lockUI.GetLockID());
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
        if (!IsLocked.Value)
        {
            ToggleOpenServerRpc();
            return; // do not open up the UI if it is unlocked
        }

        base.Interact(player);
    }

    // Function to update single digit of the current code using buttons
    public void IncrementDigit(int index)
    {
        // rebuild new combination using the old one
        Combination prevCombo = CurrentCombination.Value;
        char[] updatedValues = new char[prevCombo.Values.Length];

        // creating the new array of combination digits
        for (int i = 0; i < updatedValues.Length; i++)
        {
            if (i == index)
            {
                // Get the current character value on the lock
                char current = prevCombo.Values[i];
                // Get the index of the current character value and increment
                int curIndex = PossibleDigits.FindIndex((c) => c == current);
                int newIndex = curIndex + 1;
                // Force wrapping
                newIndex %= PossibleDigits.Count;
                // Replace letter in combination
                updatedValues[i] = PossibleDigits[newIndex];
            }
            else
            {
                // Get the current character value on the lock
                char current = prevCombo.Values[i];
                // Copy letter in combination
                updatedValues[i] = current;
            }
        }

        // replacing the old combination with the new one
        // this forces a call to the change event
        UpdateCombinationServerRpc(updatedValues);
    }

    // Function to update single digit of the current code using buttons
    public void DecrementDigit(int index)
    {
        // rebuild new combination using the old one
        Combination prevCombo = CurrentCombination.Value;
        char[] updatedValues = new char[prevCombo.Values.Length];

        // creating the new array of combination digits
        for (int i = 0; i < updatedValues.Length; i++)
        {
            if (i == index)
            {
                // Get the current character value on the lock
                char current = prevCombo.Values[i];
                // Get the index of the current character value and decrement
                int curIndex = PossibleDigits.FindIndex((c) => c == current);
                int newIndex = curIndex - 1 + PossibleDigits.Count; // need to cycle forward one full iteration of the list to avoid modulus with negative values
                // Force wrapping
                newIndex %= PossibleDigits.Count;
                // Replace letter in combination
                updatedValues[i] = PossibleDigits[newIndex];
            }
            else
            {
                // Get the current character value on the lock
                char current = prevCombo.Values[i];
                // Copy letter in combination
                updatedValues[i] = current;
            }
        }

        // replacing the old combination with the new one
        // this forces a call to the change event
        UpdateCombinationServerRpc(updatedValues);
    }

    public bool AttemptCombination()
    {
        bool result = CheckCombination();
        Debug.Log("Code resulted in: " + (result ? "Success" : "Failure"));

        // Removing the lock and updating the room
        if (result)
        {
            // Removing the lock for all players
            UnlockLockServerRpc();

            // telling the sequencer that the cabinet has been unlocked and the puzzle is completed
            if (NextClueNumber != 7)
            {
                SequenceManager.Instance.UnlockCabinetServerRpc(NextClueNumber);
            }
            // last interaction needs two cabinets and is handled differently
            else
            {
                SequenceManager.Instance.UnlockSingleCabinetServerRpc();
            }
        }

        return result;
    }

    private bool CheckCombination()
    {
        // Unlikely to ever trigger, but kept for bounds checking.
        if (LockCombination.Count != CurrentCombination.Value.Values.Length)
        {
            Debug.Log("ERROR: LOCK COMBINATION LENGTHS DO NOT MATCH");
            return false;
        }

        // Go character by character through the code and verify that they match.
        for (int i = 0; i < LockCombination.Count; i++)
        {
            if (LockCombination[i] != CurrentCombination.Value.Values[i])
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

    [ServerRpc(RequireOwnership = false)]
    private void UpdateCombinationServerRpc(char[] newValues)
    {
        CurrentCombination.Value = new Combination(newValues);
    }

    [ServerRpc(RequireOwnership = false)]
    private void UnlockLockServerRpc()
    {
        IsLocked.Value = false;
    }
}
