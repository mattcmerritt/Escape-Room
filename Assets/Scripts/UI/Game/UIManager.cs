using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine.UI;

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

    // Chat panel data
    [SerializeField] private bool ChatOpen = false; // whether chat is open or not
    [SerializeField] private ChatLogUI ChatLog;     // used to send messages
    [SerializeField] private Animator ChatAnimator; // handles the chat going hidden

    // Toggle to temporarily disable chat for other players
    [SerializeField] private bool ChatDisabled;
    [SerializeField] private TeamChatUI TeamChat;
    [SerializeField] private TeamDebriefUI TeamDebrief;

    // Primary UI
    [SerializeField] private GameObject[] PrimaryUIComponents;

    // Player controls
    [SerializeField] private PlayerMovement PlayerMovement;
    [SerializeField] private PlayerInteractions PlayerInteractions;

    // Player UI Interfaces
    [SerializeField] private List<IdentifiedUIPanel> Panels;

    // Item popup manager
    [SerializeField] private PopupUI Popups;

    // Lobby code label
    [SerializeField] private TMP_Text LobbyCodeLabel;
    [SerializeField] private GameLobby CurrentLobby;
    [SerializeField] private GameObject KeyButton;

    // Hint information
    [SerializeField] private TMP_Text HintAnnouncement;

    // Show the lobby code at the start
    private void Start()
    {
        CurrentLobby = FindObjectOfType<GameLobby>();
    }

    // checking key presses to close/open UI
    private void Update()
    {
        // Update lobby text
        LobbyCodeLabel.text = "Lobby Code:\n" + CurrentLobby.GetCurrentJoinCode();

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

        // Using enter to open/close chat
        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (ChatOpen && ChatLog.CheckChatSelected())
            {
                SendChatMessage();
            }
            else if (ChatDisabled)
            {
                if (TeamChat.CheckTeamChatSelected())
                {
                    TeamChat.SendTeamChatMessage();
                }
                // else if (TeamChat.CheckPhoneChatSelected())
                // {
                //     TeamChat.SendPhoneChatMessage();
                // }
                else if (TeamDebrief.CheckTeamChatSelected())
                {
                    TeamDebrief.SendTeamChatMessage();
                }
            }
            else if (!ChatOpen)
            {
                OpenChat();
            }
        }

        // Using escape to close menus or inventory
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (ChatOpen)
            {
                CloseChat();
            }
            if (InventoryOpen)
            {
                CloseInventory();
            }
            else if (UIPanelOpen)
            {
                if (ActiveUIPanel.GetComponent<TeamDebriefUI>() == null)
                {
                    CloseUI(ActiveUIPanel.GetComponent<UIPanel>());
                }
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

        // disable chat for special interfaces
        if (opening.GetComponent<TeamChatUI>() || opening.GetComponent<TeamDebriefUI>())
        {
            ChatDisabled = true;
            CloseChat();
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

        // disable the key button
        KeyButton.SetActive(false);

        // reseting the popup menu
        // used in the 3D object panel to show collected items
        Popups.ClosePopupImmediately();

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

        // disable chat for special interfaces
        if (closing.name == "Phone Call" || closing.name == "Debrief")
        {
            ChatDisabled = false;
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

        return true;
    }

    public bool CloseInventory()
    {
        // make sure that the inventory is open
        if (!InventoryOpen)
        {
            // Debug.LogWarning("Inventory is not open to close.");
            return false;
        }

        // updating state
        InventoryOpen = false;
        InventoryAnimator.SetTrigger("Close");

        // restoring player control if no menus are active
        // also shows primary UI again (crosshair, etc.)
        if (!ChatOpen && !InventoryOpen && !UIPanelOpen)
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

    public bool OpenChat()
    {
        if (!ChatDisabled) 
        {
            // make sure that the chat is closed
            if (ChatOpen)
            {
                Debug.LogWarning("Chat is already opened.");
                return false;
            }

            // Activating the UI
            ChatLog.OpenChat();
            ChatAnimator.SetTrigger("Show");

            // updating state
            ChatOpen = true;

            // locking the player
            PlayerMovement.LockCamera();
            PlayerInteractions.OpenMenu();

            return true;
        }
        else
        {
            return false;
        }
    }

    public bool CloseChat()
    {
        // make sure that the chat is open
        if (!ChatOpen)
        {
            Debug.LogWarning("Chat is not open to close.");
            return false;
        }

        // Activating the UI
        ChatLog.DeselectChat();
        ChatAnimator.SetTrigger("Hide");

        // updating state
        ChatOpen = false;

        // restoring player control if no menus are active
        // also shows primary UI again (crosshair, etc.)
        if (!ChatOpen && !InventoryOpen && !UIPanelOpen)
        {
            PlayerMovement.UnlockCamera();
            PlayerInteractions.CloseMenu();
        }

        return true;
    }

    public void SendChatMessage()
    {
        ChatLog.SendChatMessage();
        // CloseChat(); // Chat no longer closes when message is sent
    }

    // Method used to update the instructional text shown at the bottom of the 3D object panel
    public bool UpdatePanelInstructions(string instructions, string panelID)
    {
        // retrieve panel from list of panels
        UIPanel panel = null;
        foreach (IdentifiedUIPanel idPanel in Panels)
        {
            if (idPanel.ID == panelID)
            {
                panel = idPanel.Panel;
            }
        }

        // break if the desired panel is not present
        if (panel == null)
        {
            Debug.LogError($"Panel with ID {panelID} is missing from the list of identified panels.");
            return false;
        }

        // find the textbox and update the contents
        TMP_Text instructionsText = panel.GetComponentInChildren<TMP_Text>();
        if (instructionsText != null)
        {
            instructionsText.text = instructions;
            return true;
        }

        return false;
    }

    // Method used to update the background color of the 3D object panel for when the UV pen is in use
    public bool ChangePanelColor(string panelID, Color color)
    {
        // retrieve panel from list of panels
        UIPanel panel = null;
        foreach (IdentifiedUIPanel idPanel in Panels)
        {
            if (idPanel.ID == panelID)
            {
                panel = idPanel.Panel;
            }
        }

        // break if the desired panel is not present
        if (panel == null)
        {
            Debug.LogError($"Panel with ID {panelID} is missing from the list of identified panels.");
            return false;
        }

        // find the textbox and update the contents
        Image panelGraphic = panel.GetComponentInChildren<Image>();
        if (panelGraphic != null)
        {
            panelGraphic.color = color;
            return true;
        }

        return false;
    }

    // Method used to show the item collection popup in the 3D object panel
    public void ShowPopupPanel(string itemName, Sprite itemImage)
    {
        Popups.ShowPopup(itemName, itemImage);
    }

    public void RevealKeyButton()
    {
        KeyButton.SetActive(true);
        Button KeyButtonComponent = KeyButton.GetComponent<Button>();
        KeyButtonComponent.onClick.RemoveAllListeners();
        KeyButtonComponent.onClick.AddListener(() =>
        {
            FindObjectOfType<FakeBookWithLock>().UnlockBook();
            KeyButton.SetActive(false);
        });
    }

    public UIPanel GetActiveUIPanel() 
    {
        return ActiveUIPanel.GetComponent<UIPanel>();
    }

    public void UpdateHintAnnouncement(string hint)
    {
        HintAnnouncement.text = hint;
    }
}
