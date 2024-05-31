using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PlayerInteractions : NetworkBehaviour
{
    // Interaction constants
    [SerializeField, Range(0, 25)] private float InteractRange;

    // Camera information
    [SerializeField] private GameObject CameraObject;

    // Player state
    private bool InMenu;

    // UI Manager
    [SerializeField] private UIManager UIManager;

    // Layer for hidden players
    private LayerMask hiddenPlayers;

    // Disable this component if this player is not the owner
    public override void OnNetworkSpawn()
    {
        if (!IsOwner)
        {
            this.enabled = false;
        }
    }

    private void Update()
    {
        // ignore inputs from other players
        if (!IsOwner)
        {
            return;
        }

        if (!InMenu)
        {
            // Raycast forward to find object in front of the player
            if (Input.GetMouseButtonDown(0))
            {
                RaycastHit hit;
                if (Physics.Raycast(CameraObject.transform.position, CameraObject.transform.forward, out hit, InteractRange, 1 << hiddenPlayers))
                {
                    // Debug.Log(hit.collider.gameObject.name);
                    // Checking for components on clicked object to determine function
                    UtilityObject util = hit.collider.GetComponent<UtilityObject>();
                    SimpleObject simple = hit.collider.GetComponent<SimpleObject>();
                    ObjectChildBox child = hit.collider.GetComponent<ObjectChildBox>();
                    if (util != null)
                    {
                        util.Collect();
                    }
                    else if (simple != null && simple.enabled)
                    {
                        simple.Interact(this);
                    } 
                    else if (child != null)
                    {
                        child.ClickForParent(this);
                    }
                }
            }
        }
    }

    public void OpenWithUIManager(string panelID)
    {
        UIManager.OpenUI(panelID);
    }

    public void CloseWithUIManager(string panelID)
    {
        UIManager.CloseUI(panelID);
    }

    public void OpenItemWithUIManager(string panelID)
    {
        UIManager.OpenItemUI(panelID);
    }

    public void CloseItemWithUIManager(string panelID)
    {
        UIManager.CloseItemUI(panelID);
    }

    public void UpdatePanelInstructions(string instructions, string panelID)
    {
        UIManager.UpdatePanelInstructions(instructions, panelID);
    }

    public void ChangePanelColor(string panelID, Color color)
    {
        UIManager.ChangePanelColor(panelID, color);
    }

    // Function to prevent player from double interacting with objects if menu is open
    public void OpenMenu()
    {
        InMenu = true;
    }

    // Function to restore player interactions when menu is closed
    public void CloseMenu()
    {
        InMenu = false;
    }

    public UIManager GetUIManager()
    {
        return UIManager;
    }
}
