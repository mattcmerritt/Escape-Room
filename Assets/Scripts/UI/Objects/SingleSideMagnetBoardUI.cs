using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleSideMagnetBoardUI : MonoBehaviour
{
    [SerializeField] private List<Magnet> magnets;
    [SerializeField] private List<SlotText> slots;

    private bool complete;

    private void Update()
    {
        if (!complete)
        {
            bool allMatching = true;
            for (int i = 0; i < slots.Count; i++)
            {
                if (!slots[i].GetCurrentMagnet())
                {
                    allMatching = false;
                    break;
                }
                if (slots[i].GetSlotId() != slots[i].GetCurrentMagnet().GetMagnetId())
                {
                    allMatching = false;
                    break;
                }
            }

            if (allMatching && SequenceManager.Instance.AtMagnetBoardPuzzle())
            {
                // Sequencing to show book and indicate name was found
                SequenceManager.Instance.CompleteMagnetBoardServerRpc();
                complete = true;

                // give players clue 7
                SharedInventory inventory = FindObjectOfType<SharedInventory>();
                ClueEnvelope Clue7 = GameObject.Find("Envelope 7").GetComponent<ClueEnvelope>();

                if(inventory.CheckForItem(Clue7.ItemDetails.Name) == null)
                {
                    // add approach
                    inventory.AddItem(Clue7);

                    // show a popup
                    UIManager manager = FindObjectOfType<UIManager>();
                    manager.ShowPopupPanel(Clue7.ItemDetails.Name, Clue7.ItemDetails.Icon);
                }
            }
            else if (allMatching)
            {
                Debug.LogWarning("The magnet board was completed out of order. Waiting until the correct clue is obtained...");
            }
        }
    }

    public void MoveMagnetToSlot(int magnet, int newSlot)
    {
        magnets[magnet - 1].MoveMagnetToSlot(newSlot);
    }

    public void MoveMagnetToPosition(int magnet, Vector3 newPosition)
    {
        magnets[magnet - 1].GetComponent<RectTransform>().anchoredPosition3D = newPosition;
    }
}
