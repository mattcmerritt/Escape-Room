using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerClientData : MonoBehaviour
{
    [SerializeField] private string PlayerName;

    public void SetPlayerName(string playerName) 
    {
        PlayerName = PlayerName;
    }

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
}
