using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Conversation
{
    [CreateAssetMenu]
    public class DialogueLine : ScriptableObject
    {
        public KeywordTrigger[] PotentialTriggers; // list of triggers that need to be active for this line to appear
        public KeywordTrigger[] ExclusiveTriggers; // list of triggers that, if active alongside this one, prevent it from appearing
        
        // not really utilized
        public bool HasBeenDisplayed;

        [TextArea] public string Content;

        // used to identify a part of the interaction
        // for example, "introduction" or "address confirmation"
        public int Phase;
        public bool PhaseTransitionAfter;
        public bool CheckOutsidePhase;

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

            int ExclusiveTriggersActivated = 0;
            if(ExclusiveTriggers != null && ExclusiveTriggers.Length != 0)
            {
                foreach (KeywordTrigger trigger in ExclusiveTriggers)
                {
                    if(trigger.CheckTriggerConditions(input))
                    {
                        ExclusiveTriggersActivated += 1;
                    }
                }
                if(ExclusiveTriggersActivated == ExclusiveTriggers.Length)
                {
                    Debug.Log($"skipping {name} (exclusive trigger procced)");
                    // a more specific dialogue option exists, so don't display this one.
                    HasBeenDisplayed = true; // so it cant be displayed
                    return false;
                }
            }
            // else
            // {
            //     Debug.LogError($"ERROR: null exclusive list on {name}!");
            // }
            

            if (!triggerFailed)
            {
                Debug.Log($"showing {name} (no triggers failed)");
                HasBeenDisplayed = true;
                return true;
            }

            return false;
        }
    }
}

