using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class ClueLink : MonoBehaviour //, IPointerClickHandler
{
    [SerializeField] private TMP_Text clueText;

    // TODO: this didn't really work (always returned -1), but an alternate solution was found instead for the time being
    // [SerializeField] private Camera playerCamera;
    
    // public void OnPointerClick(PointerEventData eventData) {
    //     Vector3 clickPos = new Vector3(eventData.position.x, eventData.position.y, 0);
    //     int linkIndex = TMP_TextUtilities.FindIntersectingLink(clueText, clickPos, playerCamera); // returns -1 if no link was clicked
    //     Debug.Log(linkIndex);
    //     if(linkIndex != -1) {
    //         TMP_LinkInfo linkInfo = clueText.textInfo.linkInfo[linkIndex];
    //         string linkID = linkInfo.GetLinkID();

    //         if(linkID.Contains("https")) {
    //             Application.OpenURL(linkID);
    //         }
    //     }
    // }

}
