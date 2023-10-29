using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEditor;

public class Domino : DraggableObject
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

    [SerializeField] private bool IsRandom;
    [SerializeField] private int Top, Bottom;
    [SerializeField] private string Secret;
    [SerializeField] private TMP_Text TopText, BottomText, SecretText;
    [SerializeField] private Color LightOnPanelColor, DefaultPanelColor;

    public static List<int> UsedValues = new List<int>();

    // Player inventory information
    private SharedInventory Inventory;

    protected override void Start()
    {
        base.Start();

        // Keeping track of what sums are already present on the dominos, 
        // and generating only new sums on random dominos
        if (!UsedValues.Contains(0))
        {
            UsedValues.Add(0);
            // reserved numbers
            UsedValues.Add(22);
            UsedValues.Add(15);
            UsedValues.Add(16);
        }
        while (IsRandom && UsedValues.Contains(Top + Bottom))
        {
            Top = Random.Range(1, 12);
            Bottom = Random.Range(1, 12);
            // TODO: ask if there should be "fake" words on the back of the wrong dominos
            // Secret = Random.Range(0, 9);
            Secret = "X"; // for now, they just have an X, to demonstrate that text can still be put on them
        }
        UsedValues.Add(Top + Bottom);
        
        TopText.text = "" + Top;
        BottomText.text = "" + Bottom;
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
                    if(pen.CheckLight())
                    {
                        ObjectViewerCamera.backgroundColor = LightOnPanelColor;
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
            Domino originalDomino = Original.GetComponent<Domino>();
            if (Top != originalDomino.Top || Bottom != originalDomino.Bottom || Secret != originalDomino.Secret) 
            {
                // remove new value from domino list
                UsedValues.Remove(Top + Bottom);

                // copy values
                Top = originalDomino.Top;
                Bottom = originalDomino.Bottom;
                Secret = originalDomino.Secret;

                // update text
                TopText.text = "" + Top;
                BottomText.text = "" + Bottom;
                SecretText.text = "" + Secret;
            }
        }
    }

    public override void Interact(PlayerInteractions player) {
        base.Interact(player);
        GameObject copy = GameObject.Find("Object Viewer").GetComponentInChildren<Domino>().gameObject;
        copy.transform.localEulerAngles = new Vector3(-90f, copy.transform.localEulerAngles.y, copy.transform.localEulerAngles.z);
        copy.transform.localScale *= 2;
    }
}
