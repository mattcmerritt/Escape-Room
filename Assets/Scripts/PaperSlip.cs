using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

public class PaperSlip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IDragHandler
{
    // the textbox to display the current side
    [SerializeField] private TMP_Text Textbox;
    // font sizes for both sides
    private float FontSizeLetters = 72, FontSizeNote = 14;

    // state of the paper: one side has the note, other side has the letters for the combo
    [SerializeField] private bool ShowingNote = true;
    // contents of the note
    [SerializeField, TextArea(5, 15)] private string Letters, Note;

    // mouse information
    private bool Focused; // whether the mouse is on the UI panel

    private void Update()
    {
        // if the player right clicks on a note, it should flip over
        if (Focused && Input.GetMouseButtonDown(1))
        {
            FlipNote();
        }
    }

    // Adding the text to the note
    public void LoadNote(string letters, string note)
    {
        // save new data
        Letters = letters;
        Note = note;

        // set up textbox to show the note
        ShowingNote = true;
        Textbox.fontSize = FontSizeNote;
        Textbox.text = Note;
    }

    // Switches the contents of the paper between the note about who can prescribe and the combination
    public void FlipNote()
    {
        // switching the data displayed on the current note
        ShowingNote = !ShowingNote;
        Textbox.fontSize = ShowingNote ? FontSizeNote : FontSizeLetters;
        Textbox.text = ShowingNote ? Note : Letters;
    }

    // Detects if the mouse is in a UI element
    public void OnPointerEnter(PointerEventData eventData)
    {
        Focused = true;
    }

    // Detects if the mouse leaves a UI element
    public void OnPointerExit(PointerEventData eventData)
    {
        Focused = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector3 globalMousePos;
        if (RectTransformUtility.ScreenPointToWorldPointInRectangle(transform as RectTransform, eventData.position, eventData.pressEventCamera, out globalMousePos))
        {
            transform.position = globalMousePos;
        }
    }
}
