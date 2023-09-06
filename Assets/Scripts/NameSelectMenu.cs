using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NameSelectMenu : MonoBehaviour
{
    [SerializeField] private bool NameSet = false;
    [SerializeField] private Button ContinueButton;
    [SerializeField] private TMP_InputField PlayerNameInput;
    [SerializeField] private PlayerClientData playerClientData;

    private void UpdatePlayerName() 
    {
        playerClientData.SetPlayerName(PlayerNameInput.text);
        ContinueButton.interactable = true;
    }
}
