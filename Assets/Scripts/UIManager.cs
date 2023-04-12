using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Helper class to pair a UI panel with a unique identifier
[System.Serializable]
public class IdentifiedUIPanel
{
    public string ID;
    public UIPanel Panel;
}

public class UIManager : MonoBehaviour
{
    // UI panel data
    [SerializeField] private bool UIPanelOpen = false; // whether a UI panel is open or not
    [SerializeField] private GameObject ActiveUIPanel; // currently on screen UI panel

    // Inventory data
    [SerializeField] private bool InventoryOpen = false; // whether inventory is open or not
    [SerializeField] private Animator InventoryAnimator; // handles the clipboard going up or down

    // Primary UI
    [SerializeField] private GameObject[] PrimaryUIComponents;

    // Player controls
    [SerializeField] private PlayerMovement PlayerMovement;
    [SerializeField] private PlayerInteractions PlayerInteractions;

    // Player UI Interfaces
    [SerializeField] private List<IdentifiedUIPanel> Panels;

    // checking key presses to close/open UI
    private void Update()
    {
        // Using tab to toggle inventory
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (InventoryOpen)
            {
                CloseInventory();
            }
            else
            {
                OpenInventory();
            }
        }

        // Using escape to close menus or inventory
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (InventoryOpen)
            {
                CloseInventory();
            }
            else if (UIPanelOpen)
            {
                CloseUI(ActiveUIPanel.GetComponent<UIPanel>());
            }
        }
    }

    public bool OpenUI(UIPanel opening)
    {
        // prevent UI panels from stacking
        if (UIPanelOpen)
        {
            Debug.LogWarning($"Attempting to open {opening.gameObject.name} while another panel is active.");
            return false;
        }

        // activating the UI panel gameobject
        UIPanelOpen = true;
        ActiveUIPanel = opening.gameObject;
        ActiveUIPanel.SetActive(true);

        // locking the player
        PlayerMovement.LockCamera();
        PlayerInteractions.OpenMenu();

        // disabling primary UI
        foreach (GameObject obj in PrimaryUIComponents)
        {
            obj.SetActive(false);
        }

        return true;
    }

    // overloaded version of open UI that uses a string identifier instead of a panel reference
    // used by objects in the scene to tell tell a specific player to open a panel without access to the panel
    public bool OpenUI(string panelID)
    {
        // retrieve panel from list of panels
        UIPanel opening = null;
        foreach (IdentifiedUIPanel idPanel in Panels)
        {
            if (idPanel.ID == panelID)
            {
                opening = idPanel.Panel;
            }
        }

        // break if the desired panel is not present
        if (opening == null)
        {
            Debug.LogError($"Panel with ID {panelID} is missing from the list of identified panels.");
            return false;
        }

        return OpenUI(opening);
    }

    public bool CloseUI(UIPanel closing)
    {
        // make sure that there is a panel to close
        if (!UIPanelOpen)
        {
            Debug.LogWarning($"Attempting to close {closing.gameObject.name} while nothing is active.");
            return false;
        }
        // make sure that the panel to close is the active panel
        if (closing.name != ActiveUIPanel.name)
        {
            Debug.LogWarning($"Attempting to close {closing.gameObject.name} while {(ActiveUIPanel != null ? ActiveUIPanel.name : "NULL")} is active.");
            return false;
        }
        // make sure that the inventory is not blocking the UI panel
        if (InventoryOpen)
        {
            Debug.LogWarning($"Inventory is blocking {closing.gameObject.name} from closing.");
            return false;
        }

        // deactivating the UI panel gameobject
        UIPanelOpen = false;
        ActiveUIPanel.SetActive(false);
        ActiveUIPanel = null;

        // removing the copied 3D objects
        // needed for when the UI was an interact for a FullObject
        GameObject[] copies = GameObject.FindGameObjectsWithTag("Viewing Copy");
        for (int i = 0; i < copies.Length; i++)
        {
            Destroy(copies[i]);
        }

        // restoring player control if no menus are active
        // also shows primary UI again (crosshair, etc.)
        if (!InventoryOpen && !UIPanelOpen)
        {
            PlayerMovement.UnlockCamera();
            PlayerInteractions.CloseMenu();

            foreach (GameObject obj in PrimaryUIComponents)
            {
                obj.SetActive(true);
            }
        }

        return true;
    }

    // overloaded version of close UI that uses a string identifier instead of a panel reference
    // used by objects in the scene to tell tell a specific player to close a panel without access to the panel
    public bool CloseUI(string panelID)
    {
        // retrieve panel from list of panels
        UIPanel closing = null;
        foreach (IdentifiedUIPanel idPanel in Panels)
        {
            if (idPanel.ID == panelID)
            {
                closing = idPanel.Panel;
            }
        }

        // break if the desired panel is not present
        if (closing == null)
        {
            Debug.LogError($"Panel with ID {panelID} is missing from the list of identified panels.");
            return false;
        }

        return CloseUI(closing);
    }

    public bool OpenInventory()
    {
        // make sure that the inventory is closed
        if (InventoryOpen)
        {
            Debug.LogWarning("Inventory is already opened.");
            return false;
        }

        // updating state
        InventoryOpen = true;
        InventoryAnimator.SetTrigger("Open");

        // locking the player
        PlayerMovement.LockCamera();
        PlayerInteractions.OpenMenu();

        // disabling primary UI
        foreach (GameObject obj in PrimaryUIComponents)
        {
            obj.SetActive(false);
        }

        return false;
    }

    public bool CloseInventory()
    {
        // make sure that the inventory is open
        if (!InventoryOpen)
        {
            Debug.LogWarning("Inventory is not open to close.");
            return false;
        }

        // updating state
        InventoryOpen = false;
        InventoryAnimator.SetTrigger("Close");

        // restoring player control if no menus are active
        // also shows primary UI again (crosshair, etc.)
        if (!InventoryOpen && !UIPanelOpen)
        {
            PlayerMovement.UnlockCamera();
            PlayerInteractions.CloseMenu();

            foreach (GameObject obj in PrimaryUIComponents)
            {
                obj.SetActive(true);
            }
        }

        return false;
    }
}
