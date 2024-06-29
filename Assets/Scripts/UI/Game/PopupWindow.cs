using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PopupWindow : MonoBehaviour
{
    // Window content
    [SerializeField] private TMP_Text TitleText, DescriptionText;
    [SerializeField] private Image ItemIcon;

    // Animation data
    [SerializeField] private Animator Animator;

    public void LoadData(string itemName, Sprite itemImage)
    {
        TitleText.text = itemName;
        ItemIcon.sprite = itemImage;
    }

    public void BeginShowingWindow()
    {
        Animator.SetTrigger("Show");
    }
}
