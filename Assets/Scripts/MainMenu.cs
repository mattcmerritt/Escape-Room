using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private Animator Animator;

    public void StartGame()
    {
        // Debug.Log("Game Started");
        Animator.SetTrigger("Start");
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
