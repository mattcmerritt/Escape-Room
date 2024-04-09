using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FakeBookWithLock : DraggableObject
{
    [SerializeField] private MeshRenderer KeyHover;
    private bool FirstTimeSetup;
    private SharedInventory Inventory;
    [SerializeField] private UtilityObject[] ItemsInBook;
    [SerializeField] private InventoryItem ItemVisuals;

    protected override void Start()
    {
        base.Start();

        FirstTimeSetup = true;

        Inventory = FindObjectOfType<SharedInventory>();

        if(IsCopy)
        {
            transform.parent.localPosition = new Vector3(0, 0, -4f);
            transform.parent.Rotate(180, 0, 90);
        }
    }

    protected override void Update() {
        base.Update();

        if(FirstTimeSetup && IsCopy) {
            // KeyHover.enabled = true;
            FirstTimeSetup = false;
        }
    }

    public override void Interact(PlayerInteractions player)
    {
        base.Interact(player);

        UtilityObject keyItem = Inventory.CheckForItem("Key");
        UtilityObject bookItem = Inventory.CheckForItem(ItemsInBook[0].ItemDetails.Name);
        if(keyItem != null && bookItem == null)
        {
            UIManager manager = FindObjectOfType<UIManager>();
            manager.RevealKeyButton();
        }
    }
    
    public void UnlockBook()
    {
        UtilityObject keyItem = Inventory.CheckForItem("Key");
        UtilityObject bookItem = Inventory.CheckForItem(ItemsInBook[0].ItemDetails.Name);
        if(IsCopy && keyItem != null && bookItem == null)
        {
            // TODO: play a hinge opening animation
            foreach (UtilityObject item in ItemsInBook)
            {
                Inventory.AddItem(item);
            }

            UIManager manager = FindObjectOfType<UIManager>();
            manager.ShowPopupPanel(ItemVisuals.Name, ItemVisuals.Icon);

            // indicate that the key was used
            Inventory.MarkItemAsViewed(keyItem);
        }
    }
}
