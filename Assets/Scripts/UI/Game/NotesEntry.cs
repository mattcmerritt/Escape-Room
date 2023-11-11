using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class NotesEntry : MonoBehaviour
{
    [SerializeField] private string Content;
    [SerializeField] private bool Editing;

    [SerializeField] private string Id;

    [SerializeField] private TMP_InputField NotesContent;

    // Reference to shared notes manager
    [SerializeField] private SharedNotes SharedNotes;

    private void Start()
    {
        SharedNotes = FindObjectOfType<SharedNotes>();
    }

    public void SaveText()
    {
        Content = NotesContent.text;
    }

    // Defer removal to the SharedNotes manager
    public void RemoveNote()
    {
        SharedNotes.RemoveItem(this);
    }

    public string GetId()
    {
        return Id;
    }
}
