using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using TMPro;

[System.Serializable]
public struct PatientInformation
{
    public string informationName;
    public string value;
    public int clue;
}

public class InformationGathered : NetworkBehaviour
{
    // Values
    [SerializeField] private List<PatientInformation> patientInformationList;

    // State information
    private int currentClue;

    // Components
    [SerializeField] private TMP_Text informationPanel;

    // Singleton instance
    public static InformationGathered instance;

    // Configure singleton reference on load
    private void Start()
    {
        instance = this;

        // Load in default text
        UpdateInformationClientRpc(0);
    }

    [ClientRpc]
    public void UpdateInformationClientRpc(int clue)
    {
        string output = "Patient Information: \n\n";
        foreach (PatientInformation info in patientInformationList)
        {
            output += $"\u2022<indent=1em>{info.informationName}: {(info.clue <= currentClue ? info.value : "?")}</indent>\n\n";
        }
        informationPanel.text = output;
    }
}
