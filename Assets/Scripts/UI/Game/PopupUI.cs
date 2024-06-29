using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Unity.Netcode;

// supporting class to store the relevant information for a given item
[System.Serializable]
public class PopupInformation
{
    public string itemName;
    public Sprite itemImage;
}

public class PopupUI : NetworkBehaviour
{
    // Information to populate new windows
    [SerializeField] private GameObject PopupWindowPrefab;
    [SerializeField] private List<GameObject> QueuedPopups = new List<GameObject>();

    // List of all possible popups
    //  Done this way to allow for passing an index through RPCs
    //  Must be sorted in order to correctly display items
    [SerializeField] private List<PopupInformation> PopupInformationList;

    // Animation delays for popups
    [SerializeField] private float showDelay, hideDelay;

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
        GameObject newPopupObject = Instantiate(PopupWindowPrefab, transform);
        QueuedPopups.Add(newPopupObject);
        newPopupObject.GetComponent<PopupWindow>().LoadData(PopupInformationList[index].itemName, PopupInformationList[index].itemImage);
        StartCoroutine(AddPopupToQueue(newPopupObject));
    }

    public IEnumerator AddPopupToQueue(GameObject popup)
    {
        // wait until this popup is the first item in the popup queue
        yield return new WaitUntil(() => popup == QueuedPopups[0]);

        // play the animations and then wait out the animations to show it
        popup.GetComponent<PopupWindow>().BeginShowingWindow();
        yield return new WaitForSeconds(showDelay + hideDelay);

        // then destroy the popup
        QueuedPopups.Remove(popup);
        Destroy(popup);
    }
}
