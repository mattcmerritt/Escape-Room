using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class SharedNotes : NetworkBehaviour
{
    [SerializeField] private GameObject NotesEntryPrefab;
    [SerializeField] private List<NotesEntry> AddedNotes;

    [SerializeField] private int TotalNotesAdded;

    public GameObject AddItem(string id)
    {
        GameObject newNote = Instantiate(NotesEntryPrefab);
        NotesEntry entry = newNote.GetComponent<NotesEntry>();
        entry.SetId(id);
        AddedNotes.Add(entry);

        return newNote;
    }

    public void RemoveItem(NotesEntry note)
    {
        AddedNotes.Remove(note);
        Destroy(note.gameObject);
    }

    [ServerRpc(RequireOwnership = false)]
    public void AddNoteForAllServerRpc(ServerRpcParams serverRpcParams)
    {
        // generate id for the note
        ulong senderId = serverRpcParams.Receive.SenderClientId;
        string newId = $"{senderId}-{TotalNotesAdded}";

        AddNoteClientRpc(newId);
    }

    [ClientRpc]
    private void AddNoteClientRpc(string id)
    {
        TotalNotesAdded++;
        GameObject newNote = AddItem(id);

        InventoryUI inventoryUI = FindObjectOfType<InventoryUI>();
        inventoryUI.PlaceNoteInList(newNote);

        Debug.Log($"<color=yellow>Notes: </color>Added note with id {id}");
    }

    [ServerRpc(RequireOwnership = false)]
    public void RemoveNoteForAllServerRpc(string id)
    {
        RemoveNoteClientRpc(id);
    }

    [ClientRpc]
    private void RemoveNoteClientRpc(string id)
    {
        NotesEntry noteEntry = AddedNotes.Find((NotesEntry note) =>
        {
            return note.GetId() == id;
        });

        AddedNotes.Remove(noteEntry);
        Destroy(noteEntry.gameObject);

        Debug.Log($"<color=yellow>Notes: </color>Removed note with id {id}");
    }

    [ServerRpc(RequireOwnership = false)]
    public void UpdateNoteForAllServerRpc(string id, string newContent)
    {
        UpdateNoteClientRpc(id, newContent);
    }

    [ClientRpc]
    private void UpdateNoteClientRpc(string id, string newContent)
    {
        NotesEntry noteEntry = AddedNotes.Find((NotesEntry note) =>
        {
            return note.GetId() == id;
        });

        noteEntry.UpdateText(newContent);

        Debug.Log($"<color=yellow>Notes: </color>Update note with id {id} to say {newContent}");
    }
}
