using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FakeBookWithLock : DraggableObject
{
    [SerializeField] private MeshRenderer KeyHover;
    private bool FirstTimeSetup;
    private SharedInventory Inventory;
    [SerializeField] private UtilityObject ItemInBook;

    protected override void Start()
    {
        base.Start();

        FirstTimeSetup = true;

        Inventory = FindObjectOfType<SharedInventory>();

        if(IsCopy)
        {
            transform.parent.position += new Vector3(0, 0, 0.6f);
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
        UtilityObject bookItem = Inventory.CheckForItem(ItemInBook.ItemDetails.Name);
        if(keyItem != null && bookItem == null)
        {
            UIManager manager = FindObjectOfType<UIManager>();
            manager.RevealKeyButton();
        }
    }
    
    public void UnlockBook()
    {
        UtilityObject keyItem = Inventory.CheckForItem("Key");
        UtilityObject bookItem = Inventory.CheckForItem(ItemInBook.ItemDetails.Name);
        if(IsCopy && keyItem != null && bookItem == null)
        {
            // TODO: play a hinge opening animation
            Inventory.AddItem(ItemInBook);

            UIManager manager = FindObjectOfType<UIManager>();
            manager.ShowPopupPanel(ItemInBook.ItemDetails.Name, ItemInBook.ItemDetails.Icon);
        }
    }
}
