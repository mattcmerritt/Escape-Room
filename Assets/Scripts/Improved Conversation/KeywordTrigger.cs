using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Conversation
{
    [CreateAssetMenu]
    public class KeywordTrigger : Trigger
    {
        // Cannot have both lists populated.
        [SerializeField] private string[] RequiredWords;

        public override bool CheckTriggerConditions(string input)
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
            
            List<string> currentPlayerList = PlayerManager.Instance.ListPlayersInLobby();
            for(int i = 0; i < currentPlayerList.Count; i++)
            {
                currentPlayerList[i] = currentPlayerList[i].ToLower();
            }
            // if the word we are looking for matches a player name 
            if (currentPlayerList.Contains(unformattedWord))
            {
                return false; // TODO: can possibly make the game unbeatable, but unsure what solution should be
            }
            // TODO: this is unused
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
