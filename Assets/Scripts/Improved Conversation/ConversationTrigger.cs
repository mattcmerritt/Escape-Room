using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Conversation
{
    [CreateAssetMenu]
    public class ConversationTrigger : Trigger
    {
        public List<DialogueLine> PreceedingDialogueLines;

        // TODO: the string parameter is useless
        // it is currently only here to make is compatible with existing systems for the phone call
        public override bool CheckTriggerConditions(string input)
        {
            bool allLinesSatisfied = true;
            foreach(DialogueLine line in PreceedingDialogueLines)
            {
                if (!line.HasBeenDisplayed)
                {
                    allLinesSatisfied = false;
                }
            }
            IsTriggered = allLinesSatisfied;
            return allLinesSatisfied;
        }
    }
}
