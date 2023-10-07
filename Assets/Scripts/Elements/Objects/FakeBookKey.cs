using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class FakeBookKey : UtilityObject
{
    NetworkVariable<bool> KeyFound = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    public override void Collect()
    {
        base.Collect();
        KeyFound.Value = true;
    }

    [ServerRpc(RequireOwnership = false)]
    protected override void CollectServerRpc()
    {
        base.CollectServerRpc();
        KeyFound.Value = true;
    }

    // This item will not be usable in the main game world
    public override void Interact(PlayerInteractions player)
    {
        // does nothing, should be true already
        KeyFound.Value = true;
    }

    public bool CheckKey()
    {
        return KeyFound.Value;
    }
}
