using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class PillBox : SimpleObject
{
    [SerializeField] protected bool IsCopy; // flag used to disable interactions for copied dominos
    [SerializeField] protected PillBox Original; // for the copies, allows them to send data back to the main one
    [SerializeField] protected Vector3 ViewingCopyOffsets = Vector3.zero; // for the copies, where they should be positioned
    [SerializeField] protected Vector3 ViewingCopyRotations = Vector3.zero; // for the copies, how they should be rotated

    [SerializeField, TextArea(5, 15)] private string ItemInstructions; // the description displayed at the bottom of the panel for how to interact with the 3D object

    [SerializeField] private GameObject ObjectViewer;
    [SerializeField] private RenderTexture SampleRenderTexture;

    [SerializeField] public static Dictionary<string, bool> SlotTable;

    [SerializeField] private UtilityObject Clue;

    protected override void Start() 
    {
        if(SlotTable == null)
        {
            // get all slot objects in copy
            List<GameObject> PillBoxSlots = new List<GameObject>();

            foreach (Transform child in GetComponentsInChildren<Transform>())
            {
                if(child.gameObject.name.Contains("Pivot"))
                {
                    PillBoxSlots.Add(child.gameObject);
                }
            }

            // set up dictionary for faster lookup later when using the buttons
            SlotTable = new Dictionary<string, bool>();

            foreach (GameObject slot in PillBoxSlots)
            {
                SlotTable.Add(slot.name, false);
            }

            // TODO: remove
            // debug
            // Debug.Log(SlotTable.Keys.Count + " keys");
            // foreach (string key in SlotTable.Keys)
            // {
            //     Debug.Log("Key: " + key);
            // }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void OpenPillBoxSlotServerRpc(string name)
    {
        OpenPillBoxSlotClientRpc(name);
    }

    [ClientRpc]
    public void OpenPillBoxSlotClientRpc(string name)
    {
        if(SlotTable == null)
        {
            Debug.LogError("The table does not exist!");
        }
        else if(SlotTable.ContainsKey(name))
        {
            SlotTable[name] = true;
        }
    }

    public void OpenSpecialPillBoxSlot()
    {
        SequenceManager.Instance.OpenPillContainerServerRpc();
        Clue.Collect();
    }

    // Create a viewing copy of the object when the player enters the interact menu
    public override void Interact(PlayerInteractions player)
    {
        // if the object is a viewing copy, it cannot be interacted with
        if (IsCopy)
        {
            return;
        }

        // instantiate object viewer
        GameObject viewer = Instantiate(ObjectViewer);
        viewer.name = "Object Viewer";
        viewer.transform.position *= 1; // TODO: determine player number to make sure overlap doesnt occur

        // create render texture for player ui
        RenderTexture rt = new RenderTexture(SampleRenderTexture);
        viewer.GetComponentInChildren<Camera>().targetTexture = rt;

        // copies the object, but disables all scripts so that they are not copied over
        // to the viewing copy
        GameObject copy = Instantiate(gameObject, viewer.transform.GetChild(0)); 
        copy.transform.localPosition = ViewingCopyOffsets;
        copy.transform.eulerAngles = ViewingCopyRotations;
        copy.name = "Viewing Copy: " + copy.name;
        PillBox copyScript = copy.GetComponent<PillBox>();
        copyScript.SetAsCopy(this);

        player.UpdatePanelInstructions(ItemInstructions, PanelID);

        base.Interact(player);

        // set render texture on player ui
        player.GetComponentInChildren<RawImage>().texture = rt;
    }

    public void SetAsCopy(PillBox original)
    {
        IsCopy = true;
        Original = original;
    }

    public bool CheckIfCorrect()
    {
        return !IsCopy;
    }
}
