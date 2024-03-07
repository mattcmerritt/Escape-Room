using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class SequenceManager : NetworkBehaviour
{
    [SerializeField] private List<UtilityObject> Clues;
    [SerializeField] private SimpleObject Phone;

    [SerializeField] private int CurrentClue;

    // interaction specific tracking elements
    private Dictionary<string, bool> ScannedDominos = new Dictionary<string, bool>();
    [SerializeField] private List<GameObject> RequiredDominos;

    public static SequenceManager Instance;

    private void Start()
    {
        Instance = this;

        // disable all other clues
        foreach (UtilityObject clue in Clues)
        {
            clue.enabled = false;
            clue.gameObject.SetActive(false);
        }

        // activate the first clue
        Clues[0].enabled = true;
        Clues[0].gameObject.SetActive(true);
    }

    [ServerRpc(RequireOwnership = false)]
    public void MoveToNextClueServerRpc(int clue)
    {
        MoveToNextClueClientRpc(clue);
    }

    [ClientRpc]
    private void MoveToNextClueClientRpc(int clue)
    {
        CurrentClue += Mathf.Max(clue, CurrentClue);

        Clues[CurrentClue].enabled = true;
        Clues[CurrentClue].gameObject.SetActive(true);

        Debug.Log($"<color=blue>Sequencing:</color> Now on clue {CurrentClue}");
    }

    #region Dominos
    [ServerRpc(RequireOwnership = false)]
    public void AddToScannedDominosServerRpc(string name, bool value)
    {
        AddToScannedDominosClientRpc(name, value);
    }

    [ClientRpc]
    private void AddToScannedDominosClientRpc(string name, bool value)
    {
        ScannedDominos[name] = value;

        bool missingRequired = false;
        foreach (GameObject domino in RequiredDominos)
        {
            if (!ScannedDominos.ContainsKey(domino.name) || !ScannedDominos.GetValueOrDefault(domino.name, false))
            {
                missingRequired = true;
            }
        }

        if (!missingRequired)
        {
            MoveToNextClueServerRpc(1);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void UpdateScannedDominoServerRpc(string name)
    {
        UpdateScannedDominoClientRpc(name);
    }

    [ClientRpc]
    private void UpdateScannedDominoClientRpc(string name)
    {
        // ensure that the domino is currently active, meaning a viewing copy is present
        DotDomino[] activeDominos = FindObjectsOfType<DotDomino>();
        bool currentlyViewed = false;
        foreach (DotDomino domino in activeDominos)
        {
            if (domino.name.Contains("Viewing") && domino.name.Contains(name))
            {
                currentlyViewed = true;
            }
        }

        // only update dominos that are previously viewed and currently active
        if (ScannedDominos.ContainsKey(name) && currentlyViewed)
        {
            ScannedDominos[name] = true;
        }
    }
    #endregion Dominos
}
