using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class UVPen : UtilityObject
{
    private NetworkVariable<bool> LightActive = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    [SerializeField] private Color DefaultPanelColor, LightOnPanelColor;

    // This item will not be usable in the main game world
    public override void InteractAllClients(PlayerInteractions player)
    {
        base.InteractAllClients(player);
        LightActive.Value = !LightActive.Value;
        // player.ChangePanelColor("3D Object", LightActive.Value ? LightOnPanelColor : DefaultPanelColor);
    }

    public bool CheckLight()
    {
        return LightActive.Value;
    }
}
