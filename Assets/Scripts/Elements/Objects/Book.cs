using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Book : UtilityObject
{
    [SerializeField] private string URL;
    public override void Interact(PlayerInteractions player)
    {
        base.Interact(player); // still open panel if needed

        // Full-screen toggle
        // Screen.fullScreenMode = FullScreenMode.Windowed;

        Application.OpenURL(URL);
    }
}
