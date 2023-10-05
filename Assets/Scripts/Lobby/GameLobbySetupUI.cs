using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode.Transports.UTP;

public class GameLobbySetupUI : MonoBehaviour
{
    [SerializeField] private GameLobby GameLobby;
    [SerializeField] private GameObject MenuParent;
    [SerializeField] private UnityTransport Transport;

    public void CreateLobby()
    {
        GameLobby.CreateLobby();
        CloseUI();
    }

    private void CloseUI()
    {
        MenuParent.SetActive(false);
    }
}
