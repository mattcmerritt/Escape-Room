using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public interface IPrerequisiteTrigger
{   
    public void TriggerChange();
    public void TriggerChangeForAllServerRpc();
    public void TriggerChangeClientRpc();
}
