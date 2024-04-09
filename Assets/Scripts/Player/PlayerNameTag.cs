using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using TMPro;
using Unity.Collections;

public class PlayerNameTag : NetworkBehaviour
{
    // name stuff
    private NetworkVariable<FixedString128Bytes> Name = new NetworkVariable<FixedString128Bytes>("Unnamed Player", NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    [SerializeField] private GameObject NameTag;


    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if(IsOwner)
        {
            Name.Value = FindObjectOfType<PlayerClientData>().GetPlayerName();
        }
    }

    private void Update()
    {
        if(!IsOwner)
        {
            SetNameTag(Name.Value);
            NameTag.transform.LookAt(transform);
        }
    }

    public void SetNameTag(FixedString128Bytes name)
    {
        NameTag.GetComponent<TMP_Text>().text = "" + name.Value;
    }
}
