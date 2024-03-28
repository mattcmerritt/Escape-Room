using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PlayerAnimation : NetworkBehaviour
{
    [SerializeField] private Animator ModelAnimator;
    private NetworkVariable<bool> PlayerIsMoving = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Owner, NetworkVariableWritePermission.Owner);

    public override void OnNetworkSpawn()
    {
        PlayerIsMoving.OnValueChanged += (bool prev, bool current) => { ModelAnimator.SetBool("moving", current); };
    }

    public void ChangeMovingState(bool newValue)
    {
        PlayerIsMoving.Value = newValue;
    }
}
