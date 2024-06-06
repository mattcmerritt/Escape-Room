using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class UVPen : UtilityObject
{
    private bool LightActive;
    [SerializeField] private Color DefaultPanelColor, LightOnPanelColor;

    // This item will not be usable in the main game world, and usage should only be client-side
    public override void Interact(PlayerInteractions player)
    {
        LightActive = !LightActive;
        ChangeValueForAllServerRpc(LightActive);
        // player.ChangePanelColor("3D Object", LightActive.Value ? LightOnPanelColor : DefaultPanelColor);
    }

    [ServerRpc(RequireOwnership = false)]
    private void ChangeValueForAllServerRpc(bool value)
    {
        ChangeValueClientRpc(value);
    }

    [ClientRpc]
    private void ChangeValueClientRpc(bool value)
    {
        LightActive = value;
    }

    public bool CheckLight()
    {
        return LightActive;
    }
}
