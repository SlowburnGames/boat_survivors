using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Button = UnityEngine.UIElements.Button;

public class DialogueGraphView : GraphView
{

    public readonly Vector2 defaultNodeSize = new Vector2(150, 200);

    public DialogueGraphView()
    {
        SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);
        
        this.AddManipulator(new ContentDragger());
        this.AddManipulator(new SelectionDragger());
        this.AddManipulator(new RectangleSelector());

        AddElement(GenerateEntryNode());
    }

    private Port GeneratePort(DialogueNode node, Direction portDirection, Port.Capacity capacity = Port.Capacity.Single)
    {
        return node.InstantiatePort(Orientation.Horizontal, portDirection, capacity, typeof(float)); //type of graph does not matter
    }

    private DialogueNode GenerateEntryNode()
    {
        var node = new DialogueNode
        {
            title = "START",
            GUID = Guid.NewGuid().ToString(),
            DialogueText = "EntryPoint",
            EntryPoint = true
        };

        node.SetPosition(new Rect(100, 200, 100, 150));

        var generated_port = GeneratePort(node, Direction.Output);
        generated_port.portName = "Next";

        node.outputContainer.Add(generated_port);

        node.RefreshExpandedState();
        node.RefreshPorts();
        
        
        return node;
    }

    public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
    {
        var compatiblePorts = new List<Port>();

        ports.ForEach((port)=>{
            if(startPort != port && startPort.node != port.node)
            {
                compatiblePorts.Add(port);
            }
        });

        return compatiblePorts;
    }

    public DialogueNode CreateDialogueNode(string nodeName)
    {
        var dialogue_node = new DialogueNode
        {
            title = nodeName,
            DialogueText = nodeName,
            GUID = Guid.NewGuid().ToString()
        };

        var input_port = GeneratePort(dialogue_node, Direction.Input, Port.Capacity.Multi);
        input_port.portName = "Input";

        dialogue_node.inputContainer.Add(input_port);
        
        var add_button = new Button(()=>{AddChoicePort(dialogue_node);});
        add_button.text = "New Choice";
        dialogue_node.titleContainer.Add(add_button);


        var textField = new TextField(string.Empty);
        textField.RegisterValueChangedCallback(evt=>{
            dialogue_node.DialogueText = evt.newValue;
            dialogue_node.title = evt.newValue;
        });
        textField.SetValueWithoutNotify(dialogue_node.title);
        dialogue_node.mainContainer.Add(textField);

        dialogue_node.RefreshPorts();
        dialogue_node.SetPosition(new Rect(Vector2.zero, defaultNodeSize));
        
        return dialogue_node;
    }

    public void CreateNode(string nodeName)
    {
        AddElement(CreateDialogueNode(nodeName));
    }

    public void AddChoicePort(DialogueNode node, string overriddenPortName = "")
    {
        var generated_port = GeneratePort(node, Direction.Output);

        var oldLabel = generated_port.contentContainer.Q<Label>("type");
        generated_port.contentContainer.Remove(oldLabel);

        var outputPortCount = node.outputContainer.Query("connector").ToList().Count;

        var portName = string.IsNullOrEmpty(overriddenPortName) ? $"Choice {outputPortCount}" : overriddenPortName;

        var textField = new TextField
        {
            name = string.Empty,
            value = portName,
        };
        textField.RegisterValueChangedCallback(evt => generated_port.portName=evt.newValue);

        generated_port.contentContainer.Add(new Label("  "));
        generated_port.contentContainer.Add(textField);

        var deleteButton = new Button(()=>RemovePort(node, generated_port))
        {
            text = "X",
        };

        generated_port.contentContainer.Add(deleteButton);

        generated_port.portName = portName;
        node.outputContainer.Add(generated_port);
        node.RefreshPorts();
        node.RefreshExpandedState();
    }

    private void RemovePort(DialogueNode node, Port port)
    {
        var targetEdge = edges.ToList().Where(x=>x.output.portName == port.portName && x.output.node == port.node);

        if(!targetEdge.Any()) return;
        var edge = targetEdge.First();

        edge.input.Disconnect(edge);

        RemoveElement(targetEdge.First());

        node.outputContainer.Remove(port);
        node.RefreshPorts();
        node.RefreshExpandedState();
    }

}