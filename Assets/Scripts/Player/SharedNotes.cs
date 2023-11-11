using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class SharedNotes : NetworkBehaviour
{
    [SerializeField] private GameObject NotesEntryPrefab;
    [SerializeField] private List<NotesEntry> AddedNotes;

    [SerializeField] private int TotalNotesAdded;

    public GameObject AddItem()
    {
        GameObject newNote = Instantiate(NotesEntryPrefab);
        NotesEntry entry = newNote.GetComponent<NotesEntry>();
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

        Debug.Log($"<color=blue>Notes: </color>Added note with id {id}");
    }

    [ServerRpc(RequireOwnership = false)]
    public void RemoveNoteForAllServerRpc(string id)
    {

    }

    [ClientRpc]
    private void RemoveNoteClientRpc(string id)
    {

    }

    [ServerRpc(RequireOwnership = false)]
    public void UpdateNoteForAllServerRpc(string id, string newContent)
    {

    }

    [ClientRpc]
    private void UpdateNoteClientRpc(string id, string newContent)
    {

    }
}
