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

    // Enable the textbox
    public void EditNote()
    {
        NotesContent.interactable = true;
    }

    // Updating text and then sending that update to all clients
    public void SaveText()
    {
        Content = NotesContent.text;
        SharedNotes.UpdateNoteForAllServerRpc(Id, Content);
        NotesContent.interactable = false;
    }

    // Client method to make text reflect others
    public void UpdateText(string newContent)
    {
        Content = newContent;

        // only update the textbox if the player is not editing it
        if (!Editing)
        {
            NotesContent.text = Content;
        }
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
