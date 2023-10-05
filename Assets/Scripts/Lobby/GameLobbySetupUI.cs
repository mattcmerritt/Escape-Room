using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameLobbySetupUI : MonoBehaviour
{
    [SerializeField] private GameLobby GameLobby;
    [SerializeField] private GameObject MenuParent;
    [SerializeField] private TMP_InputField LobbyCodeInput;

    public void CreateLobby()
    {
        // GameLobby.CreateLobby();
        GameLobby.CreateRelay();
        CloseUI();
    }

    public void JoinLobby()
    {
        GameLobby.JoinRelay(LobbyCodeInput.text);
        CloseUI();
    }

    private void CloseUI()
    {
        MenuParent.SetActive(false);
    }
}
