using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UtilityObject : MonoBehaviour
{
    [SerializeField] public GameObject PhysicalObject;
    [SerializeField] public InventoryItem ItemDetails;
    protected SharedInventory Inventory;
    [SerializeField] public bool Used;

    private void Start()
    {
        Inventory = FindObjectOfType<SharedInventory>();
    }

    public void Collect()
    {
        Inventory.AddItem(this);
        PhysicalObject.SetActive(false);
    }
}
