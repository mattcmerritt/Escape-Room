using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Conversation
{
    [CreateAssetMenu]
    public class DialogueLine : ScriptableObject
    {
        public KeywordTrigger[] PotentialTriggers;
        
        public bool IsTriggered;
        public bool HasBeenDisplayed;

        [TextArea] public string Content;
        public int Priority;

        // used to identify a part of the interaction
        // for example, "introduction" or "address confirmation"
        public string AssociatedStep;
        public string[] PrerequisiteSteps;

        private void OnEnable()
        {
            IsTriggered = false;
            HasBeenDisplayed = false;
        }

        public bool CheckIfTriggered(string input)
        {
            if (IsTriggered)
            {
                return true;
            }

            foreach (KeywordTrigger trigger in PotentialTriggers)
            {
                if (trigger.CheckSpeakingConditions(input))
                {
                    IsTriggered = true;
                    return true;
                }
            }

            return false;
        }
    }
}

