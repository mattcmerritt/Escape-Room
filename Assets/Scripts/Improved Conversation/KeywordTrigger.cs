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
        [SerializeField] private string[] ProhibitedWords;

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
            if (RequiredWords.Length > 0)
            {
                // first ensure that one required word are present
                foreach (string word in RequiredWords)
                {
                    if (ContainsKeyword(input, word))
                    {
                        IsTriggered = true;
                        return true;
                    }
                }

                return false;
            }
            else
            {
                // otherwise ensure that no the banned words are present
                foreach (string word in ProhibitedWords)
                {
                    if (ContainsKeyword(input, word))
                    {
                        IsTriggered = true;
                        return true;
                    }
                }

                return false;
            }
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
