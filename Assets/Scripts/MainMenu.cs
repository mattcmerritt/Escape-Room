using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private Animator Animator;

    public void StartGame()
    {
        Debug.LogError("Game Started");
        Animator.SetTrigger("Start");
    }

    public void LoadScene()
    {
        Debug.LogError("Scene Loaded");
        SceneManager.LoadScene(1); // load the game scene with the IP select menu
    }

    public void Exit()
    {
        Debug.LogError("Game Closed");
    }
}
