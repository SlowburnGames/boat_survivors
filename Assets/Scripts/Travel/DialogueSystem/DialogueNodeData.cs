using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[Serializable]
public class DialogueNodeData
{
    public string DialogueText;
    public string NodeGUID;
    public string image;
    public Vector2 Position;
    public bool entryPoint;
    public int moraleChange = 0;
    public int resourceChange = 0;
    public string customImmediate = "";
    public bool applies_status = false;
    public string customStatus = "";
    public int duration = 0;
    public bool combat = false;
    public List<string> enemies = new List<string>();
    public int combatMoraleChange = 0;
    public int combatResourceChange = 0;
//    public DialogueNodeData(DialogueNode dialogueNode)
    //{
        //NodeGUID = dialogueNode.GUID;
        //DialogueText = dialogueNode.DialogueText;
        //image = dialogueNode.image;
        //Position = dialogueNode.GetPosition().position;
        //entryPoint = dialogueNode.EntryPoint;
        //resourceChange = dialogueNode.resourceChange;
        //moraleChange = dialogueNode.moraleChange;
        //duration = dialogueNode.duration;
        //combat = dialogueNode.combat;
    //}
}