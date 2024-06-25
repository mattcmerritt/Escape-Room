using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitSurvey : MonoBehaviour
{
    [SerializeField] private string URL;

    public void CloseToSurvey()
    {
        Application.OpenURL(URL);
        Application.Quit();
    }
}
