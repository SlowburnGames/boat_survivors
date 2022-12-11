using System;
using UnityEngine;



[Serializable]
public class DialogueNodeData
{
    public string NodeGUID;
    public string DialogueText;
    public Vector2 Position;
    public bool entryPoint;
    public int moraleChange = 0;
    public int resourceChange = 0;
    public int duration = 0;
}