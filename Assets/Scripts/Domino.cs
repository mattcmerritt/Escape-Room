using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Domino : FullObject
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
    [SerializeField] private int Top, Bottom, Secret;
    [SerializeField] private TMP_Text TopText, BottomText, SecretText;

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
            Secret = Random.Range(0, 9);
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

        // show text if UV pen is active
        UtilityObject penObject = Inventory.CheckForItem("UV Pen");
        if (penObject != null)
        {
            UVPen pen = (UVPen) penObject;
            if (pen.CheckLight())
            {
                SecretText.gameObject.SetActive(true);
            }
            else
            {
                SecretText.gameObject.SetActive(false);
            }
        }
        else
        {
            SecretText.gameObject.SetActive(false);
        }
    }
}
