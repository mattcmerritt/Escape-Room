using System.Collections;
using System.Collections.Generic;
using Palmmedia.ReportGenerator.Core;
using TMPro;
using UnityEngine;

public class ResolutionDropdown : MonoBehaviour
{
    public void HandleUpdate(int index)
    {
        FindObjectOfType<SettingsManager>().ChangeResolution(GetComponent<TMP_Dropdown>(), index);
    }
}
