using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DialogueNode
{
    public string nodeID;
    [TextArea] public string line;
    public List<DialogueChoice> choices;
}