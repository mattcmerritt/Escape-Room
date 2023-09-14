using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PlayerNetworking : NetworkBehaviour
{
    [SerializeField] private GameObject Camera, OverlayCanvas, CameraCanvas;

    public void Start()
    {
        if (!IsOwner)
        {
            Camera.SetActive(false);
            OverlayCanvas.SetActive(false);
            CameraCanvas.SetActive(false);
            // Disable this component if this player is not the owner
            this.enabled = false;
        }
    }
}
