using UnityEditor.UIElements;
using UnityEngine.UIElements;
using System;
using System.Collections.Generic;


public class ListViewWithEditableItems : VisualElement
{
    private ListView listView;

    public ListViewWithEditableItems(List<string> source, string header)
    {
        // Create the ListView
        listView = new ListView();
        listView.headerTitle = header;
        listView.selectionType = SelectionType.None;
        listView.showFoldoutHeader = true;
        listView.showAddRemoveFooter = true;

        // Bind the items to the ListView
        listView.makeItem = () => new TextField();
        listView.bindItem = (element, index) =>
        {
            var textField = (TextField)element;
            textField.value = listView.itemsSource[index] as string;
            textField.RegisterValueChangedCallback(evt => {
                listView.itemsSource[index] = evt.newValue;
            });
        };

        // Set the items source
        listView.itemsSource = source;

        // Add the ListView to the VisualElement
        Add(listView);
    }
}
