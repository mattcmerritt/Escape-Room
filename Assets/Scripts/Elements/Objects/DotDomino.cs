using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEditor;

public class DotDomino : DraggableObject
{
    /* 
     * Attempt to grid the dots to look like dominos, 
     * this will likely be a rather involved process.
     * We will need to grid the dots in a 3x3 for 1-9,
     * and in a 3x4 for 10-12.
     * In order to place them properly, they will need to
     * be anchored in the correct gridding as well.
     * However, world space UI has not proven to be the
     * most cooperative, so we will go with text numbers
     * instead for the meantime.
     * 
    [SerializeField] private GameObject DotPrefab;
    [SerializeField] private float TopMinX = 0f, TopMaxX = 0.475f, BotMinX = 0.525f, BotMaxX = 1f;
    private float SizeXTop, SizeYTop, SizeXBot, SizeYBot;
    */

    [SerializeField] private string Secret;
    [SerializeField] private TMP_Text SecretText;
    [SerializeField] private Color LightOnPanelColor, DefaultPanelColor;

    // Player inventory information
    private SharedInventory Inventory;

    protected override void Start()
    {
        base.Start();

        SecretText.text = "" + Secret;

        Inventory = FindObjectOfType<SharedInventory>();
    }

    protected override void Update()
    {
        base.Update();

        // show text and change color if UV pen is active
        UtilityObject penObject = Inventory.CheckForItem("UV Pen");
        if (penObject != null)
        {
            UVPen pen = (UVPen) penObject;

            // text on dominoes
            if (pen.CheckLight())
            {
                SecretText.gameObject.SetActive(true);
            }
            else
            {
                SecretText.gameObject.SetActive(false);
            }

            // updated for the new dominos that use an object viewer
            GameObject ObjectViewer = GameObject.FindWithTag("Viewing Copy");
            if(ObjectViewer != null)
            {
                Camera ObjectViewerCamera = ObjectViewer.GetComponentInChildren<Camera>();
                if(ObjectViewerCamera != null)
                {
                    // also checks if a dot domino so other items aren't pink
                    if(pen.CheckLight() && ObjectViewer.gameObject.GetComponentInChildren<DotDomino>() != null)
                    {
                        ObjectViewerCamera.backgroundColor = LightOnPanelColor;

                        // if pen status changes to on, update this domino to say that it is now scanned
                        // sequence manager will detect when a domino is active or not, and only trigger this when active
                        SequenceManager.Instance.UpdateScannedDominoServerRpc(gameObject.name);
                    }
                    else
                    {
                        ObjectViewerCamera.backgroundColor = DefaultPanelColor;
                    }
                }
                else
                {
                    Debug.LogError("The object viewer does not have a camera!");
                }
            }
        }
        else
        {
            SecretText.gameObject.SetActive(false);
        }

        // if it is a copy, remove the current domino from the list of dominos and steal the values from the original
        if (IsCopy && Original != null) 
        {
            DotDomino originalDomino = Original.GetComponent<DotDomino>();
            if (Secret != originalDomino.Secret) 
            {
                // copy values
                Secret = originalDomino.Secret;

                // update text
                SecretText.text = "" + Secret;
            }
        }
    }

    public override void Interact(PlayerInteractions player) {
        base.Interact(player);
        GameObject copy = GameObject.Find("Object Viewer").GetComponentInChildren<DotDomino>().gameObject;
        copy.transform.localEulerAngles = new Vector3(0f, -90f, 0f);
        copy.transform.localScale *= 2;

        // telling the sequence manager that this domino has been viewed
        UVPen pen = (UVPen) Inventory.CheckForItem("UV Pen");
        SequenceManager.Instance.AddToScannedDominosServerRpc(gameObject.name, pen != null && pen.CheckLight());
    }
}
