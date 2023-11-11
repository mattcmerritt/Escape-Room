using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class NotesEntry : MonoBehaviour
{
    [SerializeField] private string Content;
    [SerializeField] private bool Editing;

    [SerializeField] private string Id;

    [SerializeField] private TMP_InputField NotesContent;
    [SerializeField] private Button EditSaveButton;
    [SerializeField] private TMP_Text EditSaveButtonLabel;

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

        EditSaveButton.onClick.RemoveAllListeners();
        EditSaveButton.onClick.AddListener(() =>
        {
            SaveText();
        });
        EditSaveButtonLabel.text = "Save";
    }

    // Updating text and then sending that update to all clients
    public void SaveText()
    {
        Content = NotesContent.text;
        SharedNotes.UpdateNoteForAllServerRpc(Id, Content);
        NotesContent.interactable = false;

        EditSaveButton.onClick.RemoveAllListeners();
        EditSaveButton.onClick.AddListener(() =>
        {
            EditNote();
        });
        EditSaveButtonLabel.text = "Edit";
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
        SharedNotes.RemoveNoteForAllServerRpc(Id);
    }

    public string GetId()
    {
        return Id;
    }

    public void SetId(string id)
    {
        Id = id;
    }
}
