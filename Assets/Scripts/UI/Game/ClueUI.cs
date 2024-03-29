using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ClueUI : MonoBehaviour
{
    [SerializeField] private Sprite OpenSprite;

    // UI Elements
    [SerializeField] private TMP_Text ClueContent, ClueAnnouncement;

    public void UpdateClue(ClueItem clue)
    {
        ClueContent.text = clue.ClueDescription;
        ClueAnnouncement.text = clue.Announcement;
    }
}
