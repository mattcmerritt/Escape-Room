using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NameSelectMenu : MonoBehaviour
{
    [SerializeField] private Button ContinueButton;
    [SerializeField] private TMP_InputField PlayerNameInput;
    [SerializeField] private PlayerClientData playerClientData;

    public void UpdatePlayerName() 
    {
        playerClientData.SetPlayerName(PlayerNameInput.text);
        ContinueButton.interactable = true;
    }
}
