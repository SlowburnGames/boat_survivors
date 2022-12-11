using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
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

        var entry_node = CreateDialogueNode("Start", true);
        AddElement(entry_node);
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

    public DialogueNode CreateDialogueNode(string nodeName, bool entryNode = false, DialogueNodeData nodeData = null)
    {

        var dialogue_node = new DialogueNode
        {
            title = nodeName,
            DialogueText = nodeName,
            GUID = Guid.NewGuid().ToString(),
            EntryPoint = entryNode,
        };

        var input_port = GeneratePort(dialogue_node, Direction.Input, Port.Capacity.Multi);
        input_port.portName = "Input";

        dialogue_node.inputContainer.Add(input_port);
        
        var add_button = new Button(()=>{AddChoicePort(dialogue_node);});
        add_button.text = "New Choice";
        dialogue_node.titleContainer.Add(add_button);


        var entrySwitch = new UnityEngine.UIElements.Toggle("Entry Point");
        entrySwitch.RegisterValueChangedCallback(evt =>
        {
            dialogue_node.EntryPoint = evt.newValue;
        });
        entrySwitch.SetValueWithoutNotify(entryNode);
        dialogue_node.mainContainer.Add(entrySwitch);

        var textField = new TextField("Text:");
        textField.multiline = true;
        textField.RegisterValueChangedCallback(evt=>{
            dialogue_node.DialogueText = evt.newValue;
            dialogue_node.title = evt.newValue;
        });
        textField.SetValueWithoutNotify(dialogue_node.title);
        dialogue_node.mainContainer.Add(textField);

        var moraleChangeField = new IntegerField("Morale Change:");
        moraleChangeField.RegisterValueChangedCallback(evt => {
            dialogue_node.moraleChange = evt.newValue;
        });
        moraleChangeField.SetValueWithoutNotify(0);
        dialogue_node.mainContainer.Add(moraleChangeField);
        
        var resourceChangeField = new IntegerField("Resource Change:");
        resourceChangeField.RegisterValueChangedCallback(evt => {
            dialogue_node.resourceChange = evt.newValue;
        });
        resourceChangeField.SetValueWithoutNotify(0);
        dialogue_node.mainContainer.Add(resourceChangeField);

        var effectDurationField = new IntegerField("Effect Duration:");
        effectDurationField.RegisterValueChangedCallback(evt => {
            dialogue_node.duration = evt.newValue;
        });
        effectDurationField.SetValueWithoutNotify(0);
        effectDurationField.tooltip = "Values other than 0 result in adding a generic status effect.\n0: Apply change only once.\n -1: Infinite duration";
        dialogue_node.mainContainer.Add(effectDurationField);

        dialogue_node.RefreshPorts();
        dialogue_node.SetPosition(new Rect(new Vector2(100,100), defaultNodeSize));
        
        if(nodeData != null)
        {
            entrySwitch.SetValueWithoutNotify(nodeData.entryPoint);
            moraleChangeField.SetValueWithoutNotify(nodeData.moraleChange);
            resourceChangeField.SetValueWithoutNotify(nodeData.resourceChange);
            effectDurationField.SetValueWithoutNotify(nodeData.duration);
            
            dialogue_node.EntryPoint = nodeData.entryPoint;
            dialogue_node.moraleChange = nodeData.moraleChange;
            dialogue_node.resourceChange = nodeData.resourceChange;
            dialogue_node.duration = nodeData.duration;
        }

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