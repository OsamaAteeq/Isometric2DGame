using System;
using System.Collections.Generic;
using UnityEngine;
using XNode;

[CreateAssetMenu(fileName = "DialogueGraph", menuName = "Dialogue/Dialogue Graph")]
public class DialogueGraph : NodeGraph
{
    private int count = 0;
    public DialogueNode entryNode = null;

    public override Node AddNode(Type type)
    {
        Node n = base.AddNode(type);
        count++;
        n.name = n.name + " " + count.ToString();
        if (entryNode == null)
        {
            entryNode = n as DialogueNode;
            entryNode.input = entryNode;
        }
        return n;
    }

    public override void Clear()
    {
        entryNode = null;
        count = 0;
        base.Clear();
    }

    public override Node CopyNode(Node original)
    {
        Node n = base.CopyNode(original);
        count++;
        n.name = n.name + " " + count.ToString();
        return n;
    }

    public override void RemoveNode(Node node)
    {
        if (nodes.Count == 1)
        {
            count = 0;
            entryNode = null;
        }
        base.RemoveNode(node);
        if (nodes.Count == 1)
        {
            entryNode = (DialogueNode)nodes[0];
            entryNode.input = entryNode;
        }
    }
}
