using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Simple temporary class for testing events without needing a player
// DO NOT INCLUDE IN FINAL BUILDS, ONLY FOR TESTING
public class DemoManager : MonoBehaviour
{
    [SerializeField] private Cabinet LockedCabinet;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            LockedCabinet.Interact();
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            LockedCabinet.ExitInteract();
        }
    }
}
