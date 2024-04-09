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
    [SerializeField] private TMP_Text NameTagText;
    [SerializeField] private GameObject PlayerToLookAt;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if(IsOwner)
        {
            Name.Value = FindObjectOfType<PlayerClientData>().GetPlayerName();
        }

        if(!IsOwner)
        {
            PlayerNameTag[] PlayerList = FindObjectsOfType<PlayerNameTag>();	
            foreach (PlayerNameTag player in PlayerList)	
            {	
                if (player.gameObject.GetComponent<NetworkObject>().IsLocalPlayer)	
                {	
                    PlayerToLookAt = player.gameObject;	
                }	
            }
        }
    }

    private void Update()
    {
        if(!IsOwner)
        {
            SetNameTag(Name.Value);
            NameTag.transform.LookAt(PlayerToLookAt.transform);
        }
    }

    public void SetNameTag(FixedString128Bytes name)
    {
        NameTagText.text = "" + name.Value;
    }
}
