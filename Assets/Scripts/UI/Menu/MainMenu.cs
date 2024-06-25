using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private Animator Animator;
    [SerializeField] private List<GameObject> MainScreenElements;
    [SerializeField] private List<GameObject> NameScreenElements;
    [SerializeField] private List<GameObject> SettingsElements;

    public void StartGame()
    {
        // Debug.Log("Game Started");
        Animator.SetTrigger("Start");
    }

    public void ShowSettings()
    {
        // hide old menu elements
        foreach (GameObject obj in MainScreenElements)
        {
            obj.SetActive(false);
        }

        // show new settings elements
        foreach (GameObject obj in SettingsElements)
        {
            obj.SetActive(true);
        }
    }

    public void ExitSettings()
    {
        // hide old settings menu elements
        foreach (GameObject obj in SettingsElements)
        {
            obj.SetActive(false);
        }

        // show original elements
        foreach (GameObject obj in MainScreenElements)
        {
            obj.SetActive(true);
        }
    }

    public void MoveToNameInput() 
    {
        // hide old menu elements
        foreach (GameObject obj in MainScreenElements)
        {
            obj.SetActive(false);
        }

        // show new name elements
        foreach (GameObject obj in NameScreenElements)
        {
            obj.SetActive(true);
        }
    }

    public void LoadScene()
    {
        // Debug.Log("Scene Loaded");
        SceneManager.LoadScene(1); // load the game scene with the IP select menu
    }
}
