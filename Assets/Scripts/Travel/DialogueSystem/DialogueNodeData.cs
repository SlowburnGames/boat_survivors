using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[Serializable]
public class DialogueNodeData
{
    public string NodeGUID;
    public string DialogueText;
    public string image;
    public Vector2 Position;
    public bool entryPoint;
    public int moraleChange = 0;
    public int resourceChange = 0;
    public int duration = 0;
    public bool combat = false;

    public DialogueNodeData(DialogueNode dialogueNode)
    {
        NodeGUID = dialogueNode.GUID;
        DialogueText = dialogueNode.DialogueText;
        image = dialogueNode.image;
        Position = dialogueNode.GetPosition().position;
        entryPoint = dialogueNode.EntryPoint;
        resourceChange = dialogueNode.resourceChange;
        moraleChange = dialogueNode.moraleChange;
        duration = dialogueNode.duration;
        combat = dialogueNode.combat;
    }
}