using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TextItemUI : MonoBehaviour
{
    // UI Elements
    [SerializeField] private TMP_Text TextContent, Title, ClueAnnouncement;

    public void UpdateText(string content, string title)
    {
        TextContent.text = content;
        Title.text = title;
    }

    public void UpdateClue(ClueItem clue)
    {
        TextContent.text = clue.ClueDescription;
        ClueAnnouncement.text = clue.Announcement;
        Title.text = clue.Name;
    }
}
