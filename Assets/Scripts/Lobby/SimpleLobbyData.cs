using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleLobbyData
{
    public string Id, Name;
    public int Players, MaxPlayers;

    public SimpleLobbyData(string id, string name, int players, int maxPlayers)
    {
        Id = id;
        Name = name;
        Players = players;
        MaxPlayers = maxPlayers;
    }
}
