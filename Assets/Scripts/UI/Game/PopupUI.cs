using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Unity.Netcode;

[System.Serializable]
public class PopupInformation
{
    public string itemName;
    public Sprite itemImage;
}

public class PopupUI : NetworkBehaviour
{
    [SerializeField] private TMP_Text TitleText, DescriptionText;
    [SerializeField] private Image ItemIcon;

    // Animations
    [SerializeField] private Animator Animator;

    // Popups
    [SerializeField] private List<PopupInformation> PopupInformationList;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        PopupInformationList = new List<PopupInformation>();
        InventoryItem[] items = Resources.LoadAll<InventoryItem>("Scriptable Objects");
        foreach (InventoryItem item in items)
        {
            PopupInformationList.Add(new PopupInformation { itemName = item.Name, itemImage = item.Icon });
        }
        PopupInformationList.Sort((PopupInformation popup1, PopupInformation popup2) => popup1.itemName.CompareTo(popup2.itemName));
    }

    public void ShowPopup(string itemName, Sprite itemImage)
    {
        int index = PopupInformationList.FindIndex((PopupInformation p) => itemName == p.itemName && itemImage == p.itemImage);
        // Debug.Log($"Opening {itemName} with image {itemImage}, found at {index}");
        ShowPopupServerRpc(index);
    }

    [ServerRpc(RequireOwnership = false)]
    public void ShowPopupServerRpc(int index)
    {
        ShowPopupClientRpc(index);
    }

    [ClientRpc]
    public void ShowPopupClientRpc(int index)
    {
        PopupUI pop = FindObjectOfType<PopupUI>(false);
        pop.LocalShowPopup(index);
    }

    public void LocalShowPopup(int index)
    {
        TitleText.text = PopupInformationList[index].itemName;
        ItemIcon.sprite = PopupInformationList[index].itemImage;

        Animator.SetTrigger("Show");
    }

    // Helper method to hide the panel if the tab closes
    public void ClosePopupImmediately()
    {
        Animator.SetTrigger("Close Immediate");
    }
}
