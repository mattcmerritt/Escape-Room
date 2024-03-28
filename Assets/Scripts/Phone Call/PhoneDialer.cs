using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PhoneDialer : NetworkBehaviour
{
    // Phone number data
    [SerializeField] private List<int> TypedPhoneNumber;
    private int[] RequiredPhoneNumber = { 4, 7, 5, 4, 4, 1, 3, 9, 3, 6 };

    // Change events for UI to latch onto
    public Action<List<int>> OnNumberChanged;
    public Action<bool> OnNumberCalled;

    private void Start()
    {
        TypedPhoneNumber = new List<int>();
    }

    [ServerRpc(RequireOwnership = false)]
    public void EnterDigitServerRpc(int digit)
    {
        EnterDigitClientRpc(digit);
    }

    [ClientRpc]
    private void EnterDigitClientRpc(int digit)
    {
        // add number to end
        if (TypedPhoneNumber.Count < 10)
        {
            TypedPhoneNumber.Add(digit);
        }
        // if the number was full, clear the number and start again
        else
        {
            TypedPhoneNumber.Clear();
            TypedPhoneNumber.Add(digit);
        }

        OnNumberChanged?.Invoke(TypedPhoneNumber);
    }

    [ServerRpc(RequireOwnership = false)]
    public void ClearNumberServerRpc()
    {
        ClearNumberClientRpc();
    }

    [ClientRpc]
    private void ClearNumberClientRpc()
    {
        TypedPhoneNumber.Clear();

        OnNumberChanged?.Invoke(TypedPhoneNumber);
    }

    public bool CallNumber()
    {
        bool fullMatch = TypedPhoneNumber.Count == RequiredPhoneNumber.Length;
        for (int i = 0; i < RequiredPhoneNumber.Length && fullMatch; i++)
        {
            if (TypedPhoneNumber[i] != RequiredPhoneNumber[i])
            {
                fullMatch = false;
            }
        }

        // invoke event for UI callbacks
        OnNumberCalled?.Invoke(fullMatch);

        return fullMatch;
    }
}
