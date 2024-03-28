using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Conversation
{
    public abstract class Trigger : ScriptableObject
    {
        public bool IsTriggered;

        private void OnEnable()
        {
            ResetObject();
        }

        public void ResetObject()
        {
            IsTriggered = false;
        }

        public abstract bool CheckTriggerConditions(string input);
    }
}
