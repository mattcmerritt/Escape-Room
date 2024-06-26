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

    // TODO: figure out if the hidden book with area code is even needed anymore
    //  if not, where does the key go?
    // [SerializeField] private GameObject hiddenBook;

    public static SequenceManager Instance;

    // hint related information
    [TextArea, SerializeField] private List<string> Hints;
    private Coroutine CurrentWaitForHint;
    [SerializeField] private float HintDelay;

    // database export information
    private DatabaseExporter exporter;

    // object usage information
    [System.Serializable]
    public struct ItemClueAssociation
    {
        public InventoryItem item;
        public int clueNumber;
    }
    [SerializeField] private List<ItemClueAssociation> ItemClueRequirements;

    // locked objects
    [System.Serializable]
    public struct ObjectClueUnlock
    {
        public SimpleObject roomObject;
        public int clueNumber;
    }
    [SerializeField] private List<ObjectClueUnlock> ObjectClueUnlockRestrictions;
    

    private void Start()
    {
        Instance = this;

        // disable all other clues
        foreach (UtilityObject clue in Clues)
        {
            if(!clue.ItemDetails.Name.Contains("3") && !clue.ItemDetails.Name.Contains("7"))
            {
                clue.enabled = false;
                foreach (MeshRenderer renderer in clue.GetComponentsInChildren<MeshRenderer>())
                {
                    renderer.enabled = false;
                }
                clue.GetComponent<Collider>().enabled = false;
            }
        }

        // activate the first clue
        Clues[0].enabled = true;
        foreach (MeshRenderer renderer in Clues[0].GetComponentsInChildren<MeshRenderer>())
        {
            renderer.enabled = true;
        }
        Clues[0].GetComponent<Collider>().enabled = true;

        // activate the database for the server
        exporter = FindObjectOfType<DatabaseExporter>();
        exporter.ConfigureDatabaseConnection();
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
        UIManager uiMan = FindObjectOfType<UIManager>();
        uiMan.UpdateHintAnnouncement(Hints[hint]);

        Debug.Log($"<color=purple>Hints:</color> {Hints[hint]}");
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

            // update the sticky note for all players
            InventoryUI[] playerInventories = FindObjectsOfType<InventoryUI>(true);
            foreach (InventoryUI inventory in playerInventories)
            {
                inventory.UpdateInformationClientRpc(clue);
            }
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
        MoveToNextClueServerRpc(clue);
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

            foreach (ObjectClueUnlock unlock in ObjectClueUnlockRestrictions)
            {
                if (unlock.clueNumber == CurrentClue)
                {
                    unlock.roomObject.enabled = true;
                }
            }
        }
        if (clue == 7)
        {
            CurrentClue = 7;
            Phone.enabled = true;

            // TODO: possibly some note telling them to use the phone?
            Debug.Log($"<color=blue>Sequencing:</color> Now on final puzzle, the phone call (clue {CurrentClue +  1}).");
        }

        // clear out the hint UI for all clients
        UIManager uiMan = FindObjectOfType<UIManager>();
        uiMan.UpdateHintAnnouncement("");

        // mark relevant objects for a clue as complete
        foreach (ItemClueAssociation association in ItemClueRequirements)
        {
            if (association.clueNumber == CurrentClue)
            {
                association.item.IsStillNecessary = false;
            }
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
    public void FinishDSMGuideServerRpc(string panelName)
    {
        MoveToNextClueServerRpc(2);
        FinishDSMGuideClientRpc(panelName);
    }

    // Disables the address input for all players
    [ClientRpc]
    public void FinishDSMGuideClientRpc(string panelName)
    {
        PlayerInteractions[] players = FindObjectsOfType<PlayerInteractions>(false);
        foreach (PlayerInteractions player in players)
        {
            player.CloseItemWithUIManager(panelName);
        }   
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

    #region Magnet Board
    [ServerRpc(RequireOwnership = false)]
    public void CompleteMagnetBoardServerRpc()
    {
        MoveToNextClueServerRpc(6);
        // TODO: update all clients to have the correct answer
    }

    public bool AtMagnetBoardPuzzle()
    {
        Debug.Log($"at clue {CurrentClue}");
        return CurrentClue == 5;
    }
    #endregion Magnet Board

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

    // behaviour for the server to run at the end of the game
    public void EndGame()
    {
        Debug.Log("<color=yellow>Exporter:</color> The game has ended, saving data...");
        exporter.RecordResults();
    }
}
