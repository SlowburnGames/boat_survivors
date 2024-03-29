using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class DialogueGraph : EditorWindow
{
    private DialogueGraphView _graphView;
    
    private string _fileName = "New Narrative";


    [MenuItem("Graph/Dialogue Graph")]
    public static void OpenDialogueGraphWindow()
    {
        var window = GetWindow<DialogueGraph>();
        window.titleContent = new GUIContent("Dialogue Graph");

    }

    private void OnEnable()
    {
        ConstructGraphView();
        GenerateToolbar();
    }

    private void ConstructGraphView()
    {
        _graphView = new DialogueGraphView
        {
            name = "Dialogue Graph"
        };

        _graphView.StretchToParentSize();
        rootVisualElement.Add(_graphView);
    }

    private void GenerateToolbar()
    {
        var toolbar = new Toolbar();
        var nodeCreateButton = new Button(() => {
            _graphView.CreateNode("New Node");
        });

        nodeCreateButton.text = "Create Node";


        var fileNameTextField = new TextField("File Name:");
        fileNameTextField.SetValueWithoutNotify(_fileName);
        fileNameTextField.MarkDirtyRepaint();
        fileNameTextField.RegisterValueChangedCallback((evt)=>{_fileName = evt.newValue;});



        toolbar.Add(fileNameTextField);
        toolbar.Add(new Button(()=>RequestDataOperation(true)){text = "Save Data"});
        toolbar.Add(new Button(()=>RequestDataOperation(false)){text = "Load Data"});
        toolbar.Add(nodeCreateButton);


        rootVisualElement.Add(toolbar);
    }

    void RequestDataOperation(bool save)
    {
        if(string.IsNullOrEmpty(_fileName))
        {
            EditorUtility.DisplayDialog("Invalid filename!", "Please enter a valid file name.", "Sorry");
            return;
        }

        var saveUtiliy = GraphSaveUtility.GetInstance(_graphView);

        if(save)
        {
            saveUtiliy.SaveGraph(_fileName);
        }
        else
        {
            saveUtiliy.LoadGraph(_fileName);
        }
    }


    private void OnDisable()
    {
        rootVisualElement.Remove(_graphView);
    }

}
