using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ResolutionDropdown : MonoBehaviour
{
    public void HandleUpdate(int index)
    {
        FindObjectOfType<SettingsManager>().ChangeResolution(GetComponent<TMP_Dropdown>(), index);
    }
}
