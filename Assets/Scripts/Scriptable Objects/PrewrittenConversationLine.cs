using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class PrewrittenConversationLine : ScriptableObject
{
    public string PlayerContent;
    public string ResponseContent;
    public bool FailState; // if true, force a redo; if false, move forward
    public bool EndState; // if true, end the call; if false, continue
    public string SystemMessageForFail; // only exists if FailState is true
    public List<PrewrittenConversationLine> FollowUpOptions;
}