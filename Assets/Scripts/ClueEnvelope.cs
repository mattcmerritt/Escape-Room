using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ClueEnvelope : UtilityObject
{
    [SerializeField] private Sprite OpenSprite;

    // UI Elements
    [SerializeField] private TMP_Text ClueContent, ClueAnnouncement;

    public override void Interact()
    {
        base.Interact();

        ClueItem clue = (ClueItem)ItemDetails;
        ClueContent.text = clue.Description;
        ClueAnnouncement.text = clue.Announcement;
    }
}
