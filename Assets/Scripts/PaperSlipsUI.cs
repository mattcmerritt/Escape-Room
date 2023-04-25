using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PaperSlipsUI : MonoBehaviour
{
    [SerializeField] private PaperSlip[] PaperSlips;
    [SerializeField] private int SlipsLoaded = 0;

    // load a slip into the proper slot
    public void AddSlip(string letters, string note)
    {
        PaperSlips[SlipsLoaded].LoadNote(letters, note);
        PaperSlips[SlipsLoaded].gameObject.SetActive(true);

        // Debug.Log(PaperSlips[SlipsLoaded].gameObject.name);

        SlipsLoaded++;
    }
}
