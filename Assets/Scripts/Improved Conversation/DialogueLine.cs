using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Conversation
{
    [CreateAssetMenu]
    public class DialogueLine : ScriptableObject
    {
        public KeywordTrigger[] PotentialTriggers;
        
        // not really utilized
        public bool HasBeenDisplayed;

        [TextArea] public string Content;

        // used to identify a part of the interaction
        // for example, "introduction" or "address confirmation"
        public int Phase;
        public bool PhaseTransitionAfter;

        public bool FailState;
        public bool WinState;

        private void OnEnable()
        {
            ResetObject();
        }

        public void ResetObject()
        {
            HasBeenDisplayed = false;
        }

        public bool CheckIfTriggered(string input)
        {
            if (HasBeenDisplayed)
            {
                return false;
            }

            bool triggerFailed = false;
            foreach (KeywordTrigger trigger in PotentialTriggers)
            {
                if (!trigger.CheckTriggerConditions(input))
                {
                    triggerFailed = true;
                    Debug.Log($"Trigger failed: {trigger.name}");
                }
            }

            if (!triggerFailed)
            {
                HasBeenDisplayed = true;
                return true;
            }

            return false;
        }
    }
}

