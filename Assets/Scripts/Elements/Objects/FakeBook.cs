using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FakeBook : UtilityObject
{
    private SharedInventory PlayerInventory;
    [SerializeField] private UtilityObject[] ItemsInBook;

    protected override void Start()
    {
        base.Start();

        PlayerInventory = FindObjectOfType<SharedInventory>();
    }

    public override void Collect() {
        string name = "";
        foreach(UtilityObject obj in ItemsInBook)
        {
            PlayerInventory.AddItem(obj);

            // add comma if there is already something in the name string (items were added already)
            if(name != "")
            {
                name += ", ";
            }

            name += obj.ItemDetails.Name;
        }

        UIManager manager = FindObjectOfType<UIManager>();
        manager.ShowPopupPanel(name, ItemDetails.Icon);

        // Collect the object for all clients
        CollectServerRpc();
    }
}
