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

    private Port GeneratePort(DialogueNode node, Direction portDirection, Port.Capacity capacity = Port.Capacity.Multi)
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
        DialogueNode dialogue_node;
        if(nodeData == null)
        {
            dialogue_node = new DialogueNode
            {
                title = nodeName,
                DialogueText = nodeName,
                GUID = Guid.NewGuid().ToString(),
                EntryPoint = entryNode,
            };
        }
        else
        {
            dialogue_node = new DialogueNode(nodeData);
        }



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
        entrySwitch.SetValueWithoutNotify(dialogue_node.EntryPoint);
        dialogue_node.mainContainer.Add(entrySwitch);

        //------------
        //GameDisplay
        //------------

        var gameDisplayFoldout = new Foldout();
        gameDisplayFoldout.text = "Game Display";

        var textField = new TextField("Text:");
        textField.multiline = true;
        textField.RegisterValueChangedCallback(evt=>{
            dialogue_node.DialogueText = evt.newValue;
            dialogue_node.title = evt.newValue;
        });
        textField.SetValueWithoutNotify(dialogue_node.DialogueText);
        gameDisplayFoldout.contentContainer.Add(textField);

        //object field for images
        var imageField = new TextField("Image:");
        imageField.RegisterValueChangedCallback(evt =>{
            dialogue_node.image = evt.newValue;
        });
        imageField.SetValueWithoutNotify(dialogue_node.image);
        gameDisplayFoldout.contentContainer.Add(imageField);

        dialogue_node.mainContainer.Add(gameDisplayFoldout);

        //-----------------
        //Immediate Effects
        //-----------------

        var immediateEffectsFoldout = new Foldout();
        immediateEffectsFoldout.text = "Immediate Effects";
        immediateEffectsFoldout.value = false;

        var moraleChangeField = new IntegerField("Morale Change:");
        moraleChangeField.RegisterValueChangedCallback(evt => {
            dialogue_node.moraleChange = evt.newValue;
        });
        moraleChangeField.SetValueWithoutNotify(dialogue_node.moraleChange);
        immediateEffectsFoldout.contentContainer.Add(moraleChangeField);
        
        var resourceChangeField = new IntegerField("Resource Change:");
        resourceChangeField.RegisterValueChangedCallback(evt => {
            dialogue_node.resourceChange = evt.newValue;
        });
        resourceChangeField.SetValueWithoutNotify(dialogue_node.resourceChange);
        immediateEffectsFoldout.contentContainer.Add(resourceChangeField);


        var customImmediateField = new TextField("Custom Function:");
        customImmediateField.RegisterValueChangedCallback((evt) => {
            dialogue_node.customImmediate = evt.newValue;
        });
        customImmediateField.SetValueWithoutNotify(dialogue_node.customImmediate);
        immediateEffectsFoldout.contentContainer.Add(customImmediateField);

        dialogue_node.mainContainer.Add(immediateEffectsFoldout);

        //-----------------
        //Status Effects
        //-----------------

        var statusEffectsFoldout = new Foldout();
        statusEffectsFoldout.text = "Status Effect";
        statusEffectsFoldout.value = false;

        var appliesStatusField = new UnityEngine.UIElements.Toggle("Applies Status:");
        appliesStatusField.RegisterValueChangedCallback((evt) => {
            dialogue_node.applies_status = evt.newValue;
        });
        appliesStatusField.SetValueWithoutNotify(dialogue_node.applies_status);
        statusEffectsFoldout.contentContainer.Add(appliesStatusField);

        var customStatusField = new TextField("Custom Status:");
        customStatusField.RegisterValueChangedCallback((evt) => {
            dialogue_node.customStatus = evt.newValue;
        });
        customStatusField.SetValueWithoutNotify(dialogue_node.customStatus);
        statusEffectsFoldout.contentContainer.Add(customStatusField);

        var effectDurationField = new IntegerField("Effect Duration:");
        effectDurationField.RegisterValueChangedCallback(evt => {
            dialogue_node.duration = evt.newValue;
        });
        effectDurationField.SetValueWithoutNotify(dialogue_node.duration);
        effectDurationField.tooltip = "Values other than 0 result in adding a generic status effect.\n0: Apply change only once.\n -1: Infinite duration";
        statusEffectsFoldout.contentContainer.Add(effectDurationField);

        dialogue_node.mainContainer.Add(statusEffectsFoldout);

        //------
        //Combat
        //------

        var combatFoldout = new Foldout();
        combatFoldout.text = "Combat";
        combatFoldout.value = false;

        var startsCombatField = new UnityEngine.UIElements.Toggle("Starts Combat:");
        startsCombatField.RegisterValueChangedCallback((evt) => {
            dialogue_node.combat = evt.newValue;
        });
        startsCombatField.SetValueWithoutNotify(dialogue_node.combat);
        combatFoldout.contentContainer.Add(startsCombatField);


        var enemiesView = new ListView(dialogue_node.enemies);
        enemiesView.headerTitle = "Enemies";
        enemiesView.showAddRemoveFooter = true;
        enemiesView.showBoundCollectionSize = true;
        enemiesView.showFoldoutHeader = true;
        enemiesView.showBorder = true;

        combatFoldout.contentContainer.Add(enemiesView);

        var combatMoraleRewardField = new IntegerField("Morale Reward");
        combatMoraleRewardField.RegisterValueChangedCallback((evt)=>{
            dialogue_node.combatMoraleChange = evt.newValue;
        });
        combatMoraleRewardField.SetValueWithoutNotify(dialogue_node.combatMoraleChange);
        combatFoldout.contentContainer.Add(combatMoraleRewardField);


        var combatResRewardField = new IntegerField("Resource Reward");
        combatResRewardField.RegisterValueChangedCallback((evt)=>{
            dialogue_node.combatResourceChange = evt.newValue;
        });
        combatResRewardField.SetValueWithoutNotify(dialogue_node.combatResourceChange);
        combatFoldout.contentContainer.Add(combatResRewardField);

        dialogue_node.mainContainer.Add(combatFoldout);

        dialogue_node.RefreshPorts();
        dialogue_node.SetPosition(new Rect(new Vector2(100,100), defaultNodeSize));
        
        return dialogue_node;
    }
    public void CreateNode(string nodeName)
    {
        AddElement(CreateDialogueNode(nodeName));
    }

    public void AddChoicePort(DialogueNode node, string overriddenPortName = "")
    {
        //check if a port with this name already exists, if it does, do not create a port.
        foreach(VisualElement e in node.outputContainer.Children())
        {
            if(e is Port port)
            {
                if(port.portName == overriddenPortName)
                {
                    return;
                }
            }
        }

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