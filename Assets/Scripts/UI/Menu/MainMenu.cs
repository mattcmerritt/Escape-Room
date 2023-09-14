using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private Animator Animator;
    [SerializeField] private List<GameObject> MainScreenElements;
    [SerializeField] private List<GameObject> NameScreenElements;

    public void StartGame()
    {
        // Debug.Log("Game Started");
        Animator.SetTrigger("Start");
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

    public void Exit()
    {
        Application.Quit();
    }
}
