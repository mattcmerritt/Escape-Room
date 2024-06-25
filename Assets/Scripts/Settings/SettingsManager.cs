using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SettingsManager : MonoBehaviour
{
    [SerializeField] private bool Windowed = true;
    [SerializeField] private int Width = 1280, Height = 720;
    [SerializeField] private int ResolutionIndex = 0;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    public void ChangeWindowed(bool toggled)
    {
        if (toggled)
        {
            Debug.Log("<color=orange>Settings: </color> Now on Windowed.");
            Windowed = true;
        }
        else
        {
            Debug.Log("<color=orange>Settings: </color> No longer Windowed.");
            Windowed = false;
        }

        Screen.SetResolution(Width, Height, !Windowed);
    }

    public void ChangeResolution(TMP_Dropdown resolutions, int index)
    {
        string[] dimensions = resolutions.options[index].text.Split('x');
        Width = int.Parse(dimensions[0].Trim());
        Height = int.Parse(dimensions[1].Trim());
        ResolutionIndex = index;
        Debug.Log($"<color=orange>Settings: </color> Changed resolution to {Width} by {Height}.");
        Screen.SetResolution(Width, Height, !Windowed);
    }

    public void ExitGame()
    {
        // TODO: do any additional lobby things here, like removing them from the list of players
        Application.Quit();
    }

    public bool IsWindowed()
    {
        return Windowed;
    }

    public int GetResolutionIndex()
    {
        return ResolutionIndex;
    }
}
