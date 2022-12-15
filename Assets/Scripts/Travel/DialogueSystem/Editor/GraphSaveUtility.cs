using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;


public class GraphSaveUtility
{
    private DialogueGraphView _targetGraphView;

    private DialogueContainer _containerCache;

    private List<Edge> _edges => _targetGraphView.edges.ToList();
    private List<DialogueNode> _nodes => _targetGraphView.nodes.ToList().Cast<DialogueNode>().ToList();

    public static GraphSaveUtility GetInstance(DialogueGraphView targetGraphView)
    {
        return new GraphSaveUtility
        {
            _targetGraphView = targetGraphView
        };
    }

    public void SaveGraph(string filename)
    {
        //check for multiple EntryPoints
        if(_nodes.FindAll((node)=>node.EntryPoint).Count != 1)
        {
            EditorUtility.DisplayDialog("Invalid EntryPoints!", "Graphs can only have 1 entry point.", "Sorry.");
            return;
        }

//        //TODO: Save outputs without connections
        //if(!_edges.Any())
        //{
            //return;
        //}

        var dialogueContainer = ScriptableObject.CreateInstance<DialogueContainer>();

        var connectedPorts = _edges.Where(x =>x.input.node!=null).ToArray();

        for (int i = 0; i < connectedPorts.Length; i++)
        {
            var outputNode = connectedPorts[i].output.node as DialogueNode;
            var inputNode = connectedPorts[i].input.node as DialogueNode;

            dialogueContainer.nodeLinks.Add(new NodeLinkData{
                BaseNodeGuid = outputNode.GUID,
                PortName = connectedPorts[i].output.portName,
                TargetNodeGuid = inputNode.GUID
            });
        }

        foreach (var dialogueNode in _nodes)
        {
            dialogueContainer.dialogueNodeData.Add(dialogueNode.createData());
        }

        AssetDatabase.CreateAsset(dialogueContainer, $"Assets/Resources/Dialogues/{filename}.asset");
        AssetDatabase.SaveAssets();
    }

    public void LoadGraph(string filename)
    {
        _containerCache = Resources.Load<DialogueContainer>($"Dialogues/{filename}");
        if(_containerCache == null)
        {
            EditorUtility.DisplayDialog("File Not Found", "Target dialogue graph file does not exist!", "Sorry");
            return;
        }



        ClearGraph();
        CreateNodes();
        ConnectNodes();
    }

    private void ClearGraph()
    {
        foreach (var node in _nodes)
        {
            _edges.Where(x => x.input.node == node).ToList()
                .ForEach(edge => _targetGraphView.RemoveElement(edge));

            _targetGraphView.RemoveElement(node);
        }
    }

    private void CreateNodes()
    {
        foreach (var dialogueNode in _containerCache.dialogueNodeData)
        {
            var tempNode = _targetGraphView.CreateDialogueNode(dialogueNode.DialogueText, false, dialogueNode);

            tempNode.GUID = dialogueNode.NodeGUID;
            _targetGraphView.AddElement(tempNode);

            var nodePorts = _containerCache.nodeLinks.Where(x=> x.BaseNodeGuid == dialogueNode.NodeGUID).ToList();

            nodePorts.ForEach(x => _targetGraphView.AddChoicePort(tempNode, x.PortName));
        }
    }

    private void ConnectNodes()
    {
        for (var i = 0; i < _nodes.Count; i++)
        {
            var connections = _containerCache.nodeLinks.Where(x=>x.BaseNodeGuid == _nodes[i].GUID).ToList();

            for (var j = 0; j < connections.Count; j++)
            {
                var targetNodeGuid = connections[j].TargetNodeGuid;
                var targetNode = _nodes.First(x => x.GUID == targetNodeGuid);
                LinkNodes(_nodes[i].outputContainer[j].Q<Port>(), (Port) targetNode.inputContainer[0]);

                targetNode.SetPosition(new Rect(_containerCache.dialogueNodeData.First(x=> x.NodeGUID == targetNodeGuid).Position, _targetGraphView.defaultNodeSize));
            }
        }
    }

    private void LinkNodes(Port output, Port input)
    {
        var temp_edge = new Edge
        {
            input = input,
            output = output,
        };

        temp_edge?.input.Connect(temp_edge);
        temp_edge?.output.Connect(temp_edge);
        _targetGraphView.Add(temp_edge);
    }

}
