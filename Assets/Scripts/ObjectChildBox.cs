using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Helper class that is attached to child game objects of game objects that have colliders
// For example, the cabinet doors in the room have their own colliders, and the clicks need to be sent to the parent
public class ObjectChildBox : MonoBehaviour
{
    [SerializeField] private UtilityObject UtilityObject;
    [SerializeField] private SimpleObject SimpleObject;

    public void ClickForParent(PlayerInteractions player)
    {
        if (UtilityObject != null)
        {
            UtilityObject.Collect();
        }
        else
        {
            SimpleObject.Interact(player);
        }
    }
}
