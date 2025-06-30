using System.Collections.Generic;
using UnityEngine;
using XNode;

[System.Serializable]
public class DialogueNode : Node
{
    [Input] public DialogueNode input;
    [TextArea] public string line;

    [Output(dynamicPortList = true)]
    public List<DialogueChoice> choices = new List<DialogueChoice>();

    public override object GetValue(NodePort port)
    {
        return null;
    }

    public NodePort GetChoicePort(DialogueChoice choice)
    {
        int index = choices.IndexOf(choice);
        if (index < 0) return null;

        string portName = "choices " + index;
        NodePort port = GetOutputPort(portName);

        if (port != null)
        {
            if (port.ConnectionCount > 0) 
            {
                return port.Connection;
            }
            else
            {
                Debug.LogWarning($"Port \"{portName}\" has no connection, if this is not the last node, re-check the graph ");
                return null;
            }
        }
        else
        {
            Debug.LogWarning("Port not found: " + portName);
        }

        return null;
    }

    public override void OnCreateConnection(NodePort from, NodePort to)
    {
        base.OnCreateConnection(from, to);
        if (from.IsOutput)
        {
            // Allow only if there are no existing connections
            if (from.ConnectionCount > 1)
            {
                from.ClearConnections();
                from.Connect(to);
            }
        }
        
    }


}