using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

// currently, the team should only have to type the phone number once to reduce repetition

public class PhoneDialUI : MonoBehaviour
{
    [SerializeField] private GameObject DialScreen, ChatScreen;
    [SerializeField] private TMP_Text PhoneNumber;
    [SerializeField] private GameObject BadNumberText;

    private PhoneDialer PhoneDialer;

    // binding 
    private void Start()
    {
        PhoneDialer = FindObjectOfType<PhoneDialer>();

        PhoneDialer.OnNumberChanged += ShowDigits;
        PhoneDialer.OnNumberCalled += CallNumber;
    }

    public void EnterDigit(int digit)
    {
        PhoneDialer.EnterDigitServerRpc(digit);
    }

    public void ClearNumber()
    {
        PhoneDialer.ClearNumberServerRpc();
    }

    public void CallNumber()
    {
        // attempt a call and clear number if failed
        if (!PhoneDialer.CallNumber())
        {
            PhoneDialer.ClearNumberServerRpc();
        }
    }

    #region Callbacks
    // behaviors that depend on the phone dialer events
    private void CallNumber(bool result)
    {
        // show fail message
        if (!result)
        {
            BadNumberText.SetActive(true);
        }
        else
        {
            // switch to conversation screen
            DialScreen.SetActive(false);
            ChatScreen.SetActive(true);
        } 
    }

    private void ShowDigits(List<int> digits)
    {
        string number = "";
        for (int i = 0; i < digits.Count; i++)
        {
            number += digits[i];
            if (i == 2 || i == 5)
            {
                number += "-";
            }
        }

        PhoneNumber.text = number;

        // hide the fail message
        BadNumberText.SetActive(false);
    }
    #endregion Callbacks
}
