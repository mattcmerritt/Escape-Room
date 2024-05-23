using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TextItemUI : MonoBehaviour
{
    // UI Elements
    [SerializeField] private TMP_Text TextContent, ClueAnnouncement;

    public void UpdateText(string content)
    {
        TextContent.text = content;
    }

    public void UpdateClue(ClueItem clue)
    {
        TextContent.text = clue.ClueDescription;
        ClueAnnouncement.text = clue.Announcement;
    }
}
