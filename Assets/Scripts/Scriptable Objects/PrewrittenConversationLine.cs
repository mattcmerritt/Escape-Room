using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct FailInformation
{
    [TextArea(5, 15)] public string FailMessage; 
    public PrewrittenConversationLine ReturnPoint;
}

[CreateAssetMenu]
public class PrewrittenConversationLine : ScriptableObject
{
    [TextArea(5, 15)] public string PlayerContent;
    [TextArea(5, 15)] public string ResponseContent;
    public bool FailState; // if true, force a redo; if false, move forward
    public bool EndState; // if true, end the call; if false, continue
    public FailInformation FailInformation; // only exists if FailState is true
    public List<PrewrittenConversationLine> FollowUpOptions;
}