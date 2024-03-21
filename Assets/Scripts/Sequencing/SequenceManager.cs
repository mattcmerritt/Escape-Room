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
    private int UnlockedEndCabinets = 0;

    public static SequenceManager Instance;

    // hint related information
    [TextArea, SerializeField] private List<string> Hints;
    private Coroutine CurrentWaitForHint;
    [SerializeField] private float HintDelay;

    private void Start()
    {
        Instance = this;

        // disable all other clues
        foreach (UtilityObject clue in Clues)
        {
            clue.enabled = false;
            foreach (MeshRenderer renderer in clue.GetComponentsInChildren<MeshRenderer>())
            {
                renderer.enabled = false;
            }
            clue.GetComponent<Collider>().enabled = false;
        }

        // activate the first clue
        Clues[0].enabled = true;
        foreach (MeshRenderer renderer in Clues[0].GetComponentsInChildren<MeshRenderer>())
        {
            renderer.enabled = true;
        }
        Clues[0].GetComponent<Collider>().enabled = true;
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        // only the host does the hint tracking
        if (IsServer)
        {
            CurrentWaitForHint = StartCoroutine(WaitToGiveHint(0));
        }
    }

    private IEnumerator WaitToGiveHint(int clue)
    {
        Debug.Log($"$<color=blue>Sequencing:</color> Waiting {HintDelay} seconds to give hint for clue {clue + 1}.");
        yield return new WaitForSeconds(HintDelay);

        // show hint details for all clients
        DisplayHintClientRpc(clue);
    }

    [ClientRpc]
    private void DisplayHintClientRpc(int hint)
    {
        // activate some sort of hint UI for all clients
        Debug.Log(Hints[hint]);
    }

    [ServerRpc(RequireOwnership = false)]
    public void MoveToNextClueServerRpc(int clue)
    {
        if (CurrentClue == clue - 1)
        {
            // only the host does the hint tracking
            StopCoroutine(CurrentWaitForHint);
            CurrentWaitForHint = StartCoroutine(WaitToGiveHint(clue));

            MoveToNextClueClientRpc(clue);
        }
        // if a clue was somehow done out of order, start a coroutine to re-call this method once it is ready
        // important for if someone brute forces a cabinet
        else
        {
            StartCoroutine(WaitOnClueCompletion(clue));
        }
    }

    private IEnumerator WaitOnClueCompletion(int clue)
    {
        Debug.Log($"<color=blue>Sequencing:</color> Clue {clue} was completed, but not reached. Waiting...");
        yield return new WaitUntil(() => clue == CurrentClue - 1);
    }

    [ClientRpc]
    private void MoveToNextClueClientRpc(int clue)
    {
        if (clue != 7)
        {
            CurrentClue = clue;
            Clues[CurrentClue].enabled = true;
            foreach (MeshRenderer renderer in Clues[CurrentClue].GetComponentsInChildren<MeshRenderer>())
            {
                renderer.enabled = true;
            }
            Clues[CurrentClue].GetComponent<Collider>().enabled = true;
            Debug.Log($"<color=blue>Sequencing:</color> Now on clue {CurrentClue + 1}");
        }
        if (clue == 7)
        {
            CurrentClue = 7;
            Phone.enabled = true;

            // TODO: possibly some note telling them to use the phone?
            Debug.Log($"<color=blue>Sequencing:</color> Now on final puzzle, the phone call (clue {CurrentClue +  1}).");
        }
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

    #region DSM Guide
    // should only be called after check to ensure that the player is getting the right info here

    [ServerRpc(RequireOwnership = false)]
    public void PickUpDSMGuideServerRpc()
    {
        MoveToNextClueServerRpc(2);
    }
    #endregion DSM Guide

    #region Pill Bottles
    [ServerRpc(RequireOwnership = false)]
    public void UnlockCabinetServerRpc(int index)
    {
        MoveToNextClueServerRpc(index);
    }
    #endregion Pill Bottles

    #region Daily Pill Container
    [ServerRpc(RequireOwnership = false)]
    public void OpenPillContainerServerRpc()
    {
        MoveToNextClueServerRpc(5);
    }
    #endregion Daily Pill Container

    #region Medication Guide
    [ServerRpc(RequireOwnership = false)]
    public void UnlockSingleCabinetServerRpc()
    {
        UnlockedEndCabinets++;
        if (UnlockedEndCabinets >= 2)
        {
            MoveToNextClueServerRpc(7);
        }
    }
    #endregion Medication Guide
}
