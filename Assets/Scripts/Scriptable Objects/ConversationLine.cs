using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class ConversationLine : ScriptableObject
{
    public string Content;
    public string[] RequiredWords;
    public string[] ProhibitedWords;
    public ConversationLine[] ConnectedLines;
    public ConversationLine DefaultLine;

    // state checks
    public bool FailState;
    public bool WinState;

    public bool CheckIfShouldBeSpoken(string incomingMessage)
    {
        // case-insensitive
        incomingMessage = incomingMessage.ToLower();

        // verify that all the required words are present
        bool containsAllWords = true;
        foreach (string word in RequiredWords)
        {
            // check for player name appearing
            if (word == "<username>")
            {
                string playerName = FindObjectOfType<PlayerClientData>().GetPlayerName();

                if (!incomingMessage.Contains(playerName.ToLower()))
                {
                    containsAllWords = false;
                    Debug.Log($"Missing name: {word}");
                }
            }
            // check for multiple options
            else if (word.Contains("/"))
            {
                string[] options = word.Split("/");
                bool foundOne = false;
                foreach (string option in options)
                {
                    if (incomingMessage.Contains(option.ToLower()))
                    {
                        foundOne = true;
                    }

                    if (!foundOne)
                    {
                        containsAllWords = false;
                        Debug.Log($"Missing pair: {word}");
                    }
                }
            }
            else if (!incomingMessage.Contains(word.ToLower()))
            {
                containsAllWords = false;
                Debug.Log($"Missing item: {word}");
            }
        }

        // verify that all the prohibited words are not present
        bool missingAllWords = true;
        foreach (string word in ProhibitedWords)
        {
            if (word.Contains("/"))
            {
                string[] options = word.Split("/");
                bool foundOne = false;
                foreach (string option in options)
                {
                    if (incomingMessage.Contains(option.ToLower()))
                    {
                        foundOne = true;
                    }

                    if (foundOne)
                    {
                        missingAllWords = false;
                        Debug.Log($"Contains pair: {word}");
                    }
                }
            }
            else if (incomingMessage.Contains(word.ToLower()))
            {
                missingAllWords = false;
                Debug.Log($"Contains item: {word}");
            }
        }

        return containsAllWords && missingAllWords;
    }

    // returns the line that was satisfied
    public ConversationLine CheckConnectedLines(string incomingMessage)
    {
        Debug.Log($"Message given was: {incomingMessage}");

        foreach (ConversationLine response in ConnectedLines)
        {
            Debug.Log($"Checking with: {response.name}");
            if (response.CheckIfShouldBeSpoken(incomingMessage))
            {
                return response;
            }
        }

        // if none of the lines were satisfied, the response could not be understood and default line is played
        return DefaultLine;
    }
}
