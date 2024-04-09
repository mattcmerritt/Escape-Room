using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using TMPro;

public class PlayerNameTag : NetworkBehaviour
{
    // name stuff
    [SerializeField] private string Name;
    [SerializeField] private TMP_Text NameTag;

    private void Start()
    {

    }

    private void Update()
    {
        
    }

    [ServerRpc(RequireOwnership = false)]
    public void SetNameServerRpc(string name)
    {
        SetNameClientRpc(name);
    }

    [ClientRpc]
    public void SetNameClientRpc(string name)
    {
        Name = name;
        NameTag.text = name;
    }
}
