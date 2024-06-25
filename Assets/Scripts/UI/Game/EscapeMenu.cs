using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EscapeMenu : MonoBehaviour
{
    [SerializeField] private GameObject ControlsMenu, OptionsMenu;
    [SerializeField] private Sprite Folder1, Folder2;
    [SerializeField] private Image FolderBackground;
    [SerializeField] private Button ControlsButton, OptionsButton;

    public void DisplayOptions()
    {
        OptionsMenu.SetActive(true);
        ControlsMenu.SetActive(false);

        ControlsButton.interactable = true;
        OptionsButton.interactable = false;

        FolderBackground.sprite = Folder1;
    }

    public void DisplayControls()
    {
        OptionsMenu.SetActive(false);
        ControlsMenu.SetActive(true);

        ControlsButton.interactable = false;
        OptionsButton.interactable = true;

        FolderBackground.sprite = Folder2;
    }
}
