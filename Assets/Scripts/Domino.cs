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

    [SerializeField] private int Top, Bottom;
    [SerializeField] private TMP_Text TopText, BottomText;

    protected override void Start()
    {
        base.Start();

        Top = Random.Range(1, 12);
        Bottom = Random.Range(1, 12);
        TopText.text = "" + Top;
        BottomText.text = "" + Bottom;
    }
}
