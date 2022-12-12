using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class DialogueDisplay : MonoBehaviour
{
    // Start is called before the first frame update
    public DialogueContainer dialogueContainer;
    public TMP_Text description;
    public Image image;
    [SerializeField]private List<Button> buttons;
    private DialogueNodeData currentNode;
    [SerializeField]private List<NodeLinkData> currentChoices = new List<NodeLinkData>();

    [SerializeField]private TravelManager travelManager;


    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void updateText()
    {
        image.sprite = Resources.Load<Sprite>($"Dialogues/Images/{currentNode.image}");
        description.SetText(currentNode.DialogueText);
        updateButtons();
    }

    void closeEvent()
    {
        travelManager.setButtonsInteractable(true);
        gameObject.SetActive(false);
    }

    void updateButtons()
    {
        foreach (var button in buttons)
        {
            button.gameObject.SetActive(false);
            button.onClick.RemoveAllListeners();
        }

        if(currentChoices.Count == 0)
        {
            var first_button = buttons[0];
            first_button.GetComponentInChildren<TMP_Text>().SetText("Okay.");
            first_button.onClick.AddListener(closeEvent);
            first_button.gameObject.SetActive(true);
        }

        for (int i = 0; i < currentChoices.Count; i++)
        {
            var textmesh = buttons[i].GetComponentInChildren<TMP_Text>();
            var choice = currentChoices[i];
            textmesh.SetText(choice.PortName);
            buttons[i].gameObject.SetActive(true);
            buttons[i].onClick.AddListener(()=>traverse(choice));
        }
    }

    void applyEffects()
    {
        if(currentNode.duration == 0)
        {
            travelManager.addMorale(currentNode.moraleChange);
            travelManager.addRes(currentNode.resourceChange);
        }
        else if(currentNode.duration == -1)
        {
            travelManager.addStatus(new GenericStatus(travelManager, currentNode.moraleChange, currentNode.resourceChange, 1, true));
        }
        else
        {
            travelManager.addStatus(new GenericStatus(travelManager, currentNode.moraleChange, currentNode.resourceChange, currentNode.duration));
        }

    }

    void traverse(NodeLinkData edge)
    {
        currentNode = dialogueContainer.dialogueNodeData.Find((node)=>node.NodeGUID == edge.TargetNodeGuid);
        currentChoices = getChoices();
        applyEffects();
        updateText();
    }


    public void init()
    {
        currentNode = dialogueContainer.dialogueNodeData.Find((node)=>node.entryPoint);
        currentChoices = getChoices();
        applyEffects();
        updateText();
    }

    private List<NodeLinkData> getChoices()
    {
        var choices = dialogueContainer.nodeLinks.FindAll((edge)=>edge.BaseNodeGuid == currentNode.NodeGUID);

        return choices;
    }
}
