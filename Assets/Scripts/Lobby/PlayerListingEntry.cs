using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerListingEntry : MonoBehaviour
{
    [SerializeField] private TMP_Text PlayerName;
    [SerializeField] private Toggle IsObserverToggle;

    private GameLobby GameLobby;

    private void Start()
    {
        GameLobby = FindObjectOfType<GameLobby>();
    }

    public void Initialize(string playerName, bool isHost, bool isObserver, string playerId, bool isYou)
    {
        PlayerName.text = playerName + (isHost ? " (Host)" : "") + (isYou ? " (You)" : "");

        // update the toggle value to reflect current status and ownership
        IsObserverToggle.isOn = isObserver;
        IsObserverToggle.interactable = isYou;

        // add listener to call function in GameLobby to update the player data if current player
        if (isYou)
        {
            IsObserverToggle.onValueChanged.AddListener((bool value) =>
            {
                Debug.Log("<color=white>Lobby Screen:</color> Toggle was pressed for " + playerName);
                GameLobby.UpdatePlayerObserverStatus(value);
            });
        }
    }
}
