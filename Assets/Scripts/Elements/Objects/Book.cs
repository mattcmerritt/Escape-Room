using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Book : UtilityObject
{
    [SerializeField] private string URL;
    [SerializeField] private string ConfirmationPanelName;
    public override void Interact(PlayerInteractions player)
    {
        // Full-screen toggle
        // Screen.fullScreenMode = FullScreenMode.Windowed;

        Application.OpenURL(URL);

        if (ConfirmationPanelName != null && ConfirmationPanelName != "")
        {
            player.OpenWithUIManager(ConfirmationPanelName);
        }
    }
}
