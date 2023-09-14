using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PopupUI : MonoBehaviour
{
    [SerializeField] private TMP_Text TitleText, DescriptionText;
    [SerializeField] private Image ItemIcon;

    // Animations
    [SerializeField] private Animator Animator;

    public void ShowPopup(string itemName, Sprite itemImage)
    {
        TitleText.text = itemName;
        ItemIcon.sprite = itemImage;

        Animator.SetTrigger("Show");
    }

    // Helper method to hide the panel if the tab closes
    public void ClosePopupImmediately()
    {
        Animator.SetTrigger("Close Immediate");
    }
}
