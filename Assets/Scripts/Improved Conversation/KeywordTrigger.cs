using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Conversation
{
    [CreateAssetMenu]
    public class KeywordTrigger : ScriptableObject
    {
        // Cannot have both lists populated.
        [SerializeField] private string[] RequiredWords;

        public bool IsTriggered;

        private void OnEnable()
        {
            ResetObject();
        }

        public void ResetObject()
        {
            IsTriggered = false;
        }

        public bool CheckTriggerConditions(string input)
        {
            if (IsTriggered) 
            {
                return true; // condition was met in a previous message
            }

            bool ConditionsMet = false;

            if (RequiredWords.Length > 0)
            {
                // ensure that one required word is present
                foreach (string word in RequiredWords)
                {
                    if (ContainsKeyword(input, word))
                    {
                        ConditionsMet = true;
                        IsTriggered = true;
                    }
                }
            }

            return ConditionsMet;
        }

        private bool ContainsKeyword(string input, string word)
        {
            string unformattedWord = word.ToLower();
            string unformattedInput = input.ToLower();

            // check for player name
            if (unformattedWord == "<username>")
            {
                string playerName = FindObjectOfType<PlayerClientData>().GetPlayerName();

                return unformattedInput.Contains(playerName.ToLower());
            }
            // multiple different options for a single trigger
            else if (unformattedWord.Contains("/"))
            {
                string[] options = unformattedWord.Split("/");
                bool foundOne = false;
                foreach (string option in options)
                {
                    if (unformattedInput.Contains(option))
                    {
                        foundOne = true;
                    }
                }

                return foundOne;
            }
            // default exact word check
            else
            {
                return unformattedInput.Contains(unformattedWord);
            }
        }
    }
}
