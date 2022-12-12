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
        duration = dialogueNodeData.duration;
        combat = dialogueNodeData.combat;
    }

    public DialogueNode() : base()
    {
        
    }
}
