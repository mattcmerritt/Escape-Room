using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Conversation
{
    [CreateAssetMenu]
    public class KeywordTrigger : ScriptableObject
    {
        [SerializeField] private string[] RequiredWords;
        [SerializeField] private string[] ProhibitedWords;

        public bool CheckSpeakingConditions(string input)
        {
            // first ensure that all required words are present
            bool hasNeededWords = true;
            foreach (string word in RequiredWords)
            {
                if (!ContainsKeyword(input, word))
                {
                    hasNeededWords = false;
                }
            }

            // second ensure that all the banned words are not present
            bool hasProhibitedWords = false;
            foreach (string word in ProhibitedWords)
            {
                if (ContainsKeyword(input, word))
                {
                    hasProhibitedWords = true;
                }
            }

            return hasNeededWords && !hasProhibitedWords;
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
