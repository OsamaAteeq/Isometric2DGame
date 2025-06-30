using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DialogueTree", menuName = "Scriptable Objects/DialogueTree")]
public class DialogueTree : ScriptableObject
{
    public string entryNodeID;
    public List<DialogueNode> nodes;

    public DialogueNode GetNodeByID(string id)
    {
        if (id == null || id.Trim() == "")
        {
            return null;
        }
        else
        {
            return nodes.Find(n => n.nodeID == id);
        }
    }
}
