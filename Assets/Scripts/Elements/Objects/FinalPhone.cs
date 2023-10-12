using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinalPhone : SimpleObject
{
    // Phone number
    [SerializeField] private string PhoneNumber;

    // Lists of keywords to search for at each point
    [SerializeField] private List<string> IntroductionKeywords, NameCollectionKeywords, NamesGivenKeywords, PickUpKeywords;
    [SerializeField, TextArea(4, 15)] private string IntroductionMessage, NamesGivenMessage, PickUpMessage;

    private void Start()
    {
        base.Start();

        IntroductionKeywords = new List<string>();
        NameCollectionKeywords = new List<string>();
        NamesGivenKeywords = new List<string>();
        PickUpKeywords = new List<string>();

        // Setup data
        PhoneNumber = "4754413936";

        // These lists will be used to check if the player has included any phrases that indicate that
        // they should move on with the conversation.

        // First question that the player needs to respond to is to introduce themselves.
        // They should say their name or something about their position.
        IntroductionKeywords.Add("my name is");
        IntroductionKeywords.Add("this is");
        IntroductionKeywords.Add("from the hospital");
        IntroductionKeywords.Add("doctor");

        // Second question that the player needs to respond to is to figure out who they are talking to.
        // They should confirm this person's identity before sharing any details.
        NameCollectionKeywords.Add("who am i speaking with");
        NameCollectionKeywords.Add("what is your name");
        NameCollectionKeywords.Add("who are you");

        // Third question is that the player needs to state that they have Mark Roth.
        NamesGivenKeywords.Add("mark roth");

        // Fourth question is that the player needs to make sure that the person on the phone is coming
        // to pick up Mark.
        PickUpKeywords.Add("come get");
        PickUpKeywords.Add("pick up");
    }
}
