using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FakeBook : DraggableObject
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
    }

    protected override void Update() {
        base.Update();

        if(FirstTimeSetup && IsCopy) {
            // KeyHover.enabled = true;
            FirstTimeSetup = false;
        }

        UtilityObject keyItem = Inventory.CheckForItem("Key");
        if(IsCopy && keyItem != null)
        {
            FakeBookKey key = (FakeBookKey) keyItem;
            if(Input.GetMouseButtonDown(1)) 
            {
                // TODO: play a hinge opening animation
                Inventory.AddItem(ItemInBook);

                UIManager manager = FindObjectOfType<UIManager>();
                manager.ShowPopupPanel(ItemInBook.ItemDetails.Name, ItemInBook.ItemDetails.Icon);

                Debug.Log("Click with key");
            }
        }
    }
}
