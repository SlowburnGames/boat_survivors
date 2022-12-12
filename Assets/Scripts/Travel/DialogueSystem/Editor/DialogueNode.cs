using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Experimental.GraphView;


public class DialogueNode : Node
{
    public string GUID;
    public string DialogueText;
    public string image;
    public bool EntryPoint = false;
    public int moraleChange = 0;
    public int resourceChange = 0;
    public string customImmediate = "";
    public bool applies_status = false;
    public string customStatus = "";
    public int duration = 0;
    public bool combat = false;

    public DialogueNode(DialogueNodeData dialogueNodeData) : base()
    {
        GUID = dialogueNodeData.NodeGUID;
        DialogueText = dialogueNodeData.DialogueText;
        image = dialogueNodeData.image;
        EntryPoint = dialogueNodeData.entryPoint;
        moraleChange = dialogueNodeData.moraleChange;
        resourceChange = dialogueNodeData.resourceChange;
        customImmediate = dialogueNodeData.customImmediate;
        applies_status = dialogueNodeData.applies_status;
        customStatus = dialogueNodeData.customStatus;
        duration = dialogueNodeData.duration;
        combat = dialogueNodeData.combat;
    }

    public DialogueNode() : base()
    {
        
    }

    public DialogueNodeData createData()
    {
        return new DialogueNodeData{
            NodeGUID = GUID,
            DialogueText = DialogueText,
            image = image,
            Position = this.GetPosition().position,
            entryPoint = EntryPoint,
            resourceChange = resourceChange,
            moraleChange = moraleChange,
            customImmediate = customImmediate,
            applies_status = applies_status,
            customStatus = customStatus,
            duration = duration,
            combat = combat,
        };
    }
}
