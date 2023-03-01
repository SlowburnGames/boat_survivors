using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class DialogueDisplay : MonoBehaviour
{
    public static DialogueDisplay instance { get; private set; }

    // Start is called before the first frame update
    public DialogueContainer dialogueContainer;
    public TMP_Text description;
    [SerializeField]private List<Button> buttons;
    [SerializeField]public List<List<NodeLinkData>> button_choices;
    private DialogueNodeData currentNode;
    [SerializeField]private List<NodeLinkData> currentChoices = new List<NodeLinkData>();

    [SerializeField]private TravelManager travelManager;

    private void Awake() {
        instance = this;
    }

    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void updateText()
    {
        description.SetText(currentNode.DialogueText);
        updateButtons();
    }

    public void closeEvent()
    {
        travelManager.setButtonsInteractable(true);
        transform.GetChild(0).gameObject.SetActive(false);
    }

    void updateButtons()
    {
        foreach (var button in buttons)
        {
            button.gameObject.SetActive(false);
            button.onClick.RemoveAllListeners();
            button.GetComponentInChildren<TMP_Text>().SetText("");
        }

        button_choices = new List<List<NodeLinkData>>();

        for (int i = 0; i < 4; i++)
        {
            button_choices.Add(new List<NodeLinkData>());
        }


        if(currentChoices.Count == 0)
        {
            var first_button = buttons[0];
            first_button.GetComponentInChildren<TMP_Text>().SetText("Okay.");
            first_button.GetComponent<DialogueButton>().ends_dialogue = true;
            first_button.gameObject.SetActive(true);
            if(currentNode.combat)
            {
                first_button.GetComponent<DialogueButton>().starts_combat = true;
                first_button.GetComponent<DialogueButton>().ends_dialogue = false;
                first_button.GetComponentInChildren<TMP_Text>().SetText("Fight.");
            }
            return;
        }

        int button_index = 0;

        foreach(NodeLinkData choice in currentChoices)
        {
            bool button_needed = true;
            for(int j=0; j < buttons.Count; j++)
            {
                var button_tmp = buttons[j].GetComponentInChildren<TMP_Text>();
                if(button_tmp.text == choice.PortName)
                {
                    button_choices[j].Add(choice);
                    button_needed = false;
                }
            }
            if(button_needed)
            {
                var textmesh = buttons[button_index].GetComponentInChildren<TMP_Text>();
                Debug.Log(button_index);
                textmesh.SetText(choice.PortName);
                buttons[button_index].gameObject.SetActive(true);
                button_choices[button_index].Add(choice);
                button_index++;
            }
        }
    }

    void applyEffects()
    {
        if(currentNode.duration == 0)
        {
            GameManager.Instance.addMorale(currentNode.moraleChange);
            GameManager.Instance.addRes(currentNode.resourceChange);
        }
        else if(currentNode.duration == -1)
        {
            GameManager.Instance.addStatus(new GenericStatus(currentNode.moraleChange, currentNode.resourceChange, 1, true));
        }
        else
        {
            GameManager.Instance.addStatus(new GenericStatus(currentNode.moraleChange, currentNode.resourceChange, currentNode.duration));
        }
        if(currentNode.applies_status)
        {
            var type = Type.GetType(currentNode.customStatus);
            GameManager.Instance.addStatus(Activator.CreateInstance(type) as StatusEffect);
        }
    }

    void traverse(NodeLinkData edge)
    {
        currentNode = dialogueContainer.dialogueNodeData.Find((node)=>node.NodeGUID == edge.TargetNodeGuid);
        currentChoices = getChoices();
        applyEffects();
        updateText();
        TravelManager.instance.updateUI();
    }

    public void buttonPress(int index)
    {
        Debug.Log(index);
        Debug.Log(button_choices.Count);
        List<NodeLinkData> list = button_choices[index];

        //get random outcome for button
        int random = UnityEngine.Random.Range(0, list.Count);
        Debug.Log(random);
        NodeLinkData edge = list[random];

        traverse(edge);
    }

    public void startCombat()
    {
        if(currentNode.combat)
        {
            GameManager.Instance.setCombatRewards(currentNode.combatMoraleChange, currentNode.combatResourceChange);
            GameManager.Instance.startCombat(currentNode.enemies);
        }
    }

  public void init()
    {
        currentNode = dialogueContainer.dialogueNodeData.Find((node)=>node.entryPoint);
        currentChoices = getChoices();
        applyEffects();
        updateText();
        transform.GetChild(0).gameObject.SetActive(true);
    }

    private List<NodeLinkData> getChoices()
    {
        var choices = dialogueContainer.nodeLinks.FindAll((edge)=>edge.BaseNodeGuid == currentNode.NodeGUID);

        return choices;
    }
}
