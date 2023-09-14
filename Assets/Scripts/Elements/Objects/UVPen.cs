using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UVPen : UtilityObject
{
    private bool LightActive;
    [SerializeField] private Color DefaultPanelColor, LightOnPanelColor;

    // This item will not be usable in the main game world
    public override void Interact(PlayerInteractions player)
    {
        LightActive = !LightActive;
        player.ChangePanelColor("3D Object", LightActive ? LightOnPanelColor : DefaultPanelColor);
    }

    public bool CheckLight()
    {
        return LightActive;
    }
}
