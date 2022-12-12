using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class TravelManager : MonoBehaviour
{
    [Header("Resources/Resource Display")]
    public int population = 3;
    [SerializeField]private int morale = 50;
    [SerializeField]private int resources = 50;
    [SerializeField]public RessourceDisplay resource_display;
    [SerializeField]private Button[] buttons;

    [Header("Dialogue System")]
    [SerializeField]private List<DialogueContainer> events;
    private int currentEventIndex = 0;
    public DialogueDisplay dialogueDisplay;

    [Header("Time, Day/Night Cycle")]
    private GameObject time_display;
    [SerializeField]public Sprite sun_sprite;
    [SerializeField]public Sprite moon_sprite;
    private bool isDay = true;
    [Header("Boat")]
    public int travel_distance = 0;
    private Animator boat_animator;

    [Header("Status Effects")]
    private List<StatusEffect> active_status_effects = new List<StatusEffect>();


    // Start is called before the first frame update
    void Start()
    {
        loadRandomEvents();
        setupCamera();
        updateResUI();
        Transform canvas = transform.Find("Canvas");
        Transform time_display_transform = canvas.Find("TimeDisplay");
        time_display = time_display_transform.gameObject;
        Transform boat = canvas.Find("Boat");
        boat_animator = boat.GetComponent<Animator>();
    }

    void loadRandomEvents()
    {
        var cache = Resources.LoadAll("Dialogues", typeof(DialogueContainer));

        foreach (var item in cache)
        {
            events.Add(item as DialogueContainer);
        }

        var count = events.Count;
        var last = count - 1;
        for (var i = 0; i < last; ++i) {
            var r = UnityEngine.Random.Range(i, count);
            var tmp = events[i];
            events[i] = events[r];
            events[r] = tmp;
        }

    }

    void setupCamera()
    {
        Camera.main.orthographicSize = 100 * Screen.height / Screen.width * 0.5f;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void addStatus(StatusEffect statusEffect)
    {
        active_status_effects.Add(statusEffect);
    }


    public void sailButton()
    {
        //set animations for sailing
        //wait for some time 2 seconds or so
        setButtonsInteractable(false);
        boat_animator.Play("boat_travel");
        Invoke("sail", 5);
    }

    private void sail()
    {
        //time goes on
        switchTime();
        consumeResources();
        checkLose();
        applyStatusEffects();
        startRandomEvent();

    }

    private void switchTime()
    {
        isDay = !isDay;

        if(isDay)
        {
            time_display.GetComponent<Image>().sprite = sun_sprite;
        }
        else
        {
            time_display.GetComponent<Image>().sprite = moon_sprite;
        }

    }

    public void setButtonsInteractable(bool interactable)
    {
        foreach (Button button in buttons)
        {
            button.interactable = interactable;
        }
    }

    void applyStatusEffects()
    {
        List<StatusEffect> to_remove = new List<StatusEffect>();
        foreach (var status in active_status_effects)
        {
            status.applyStatusTick();
            if(status._duration == 0)
            {
                to_remove.Add(status);
            }
        }
        foreach (var status in to_remove)
        {
            active_status_effects.Remove(status);
        }
    }


    private void consumeResources()
    {
        if(resources > 0)
        {
            //TODO: change depending on amount of people on the ship
            addRes(population * -2);
        }
        else //if resources are empty use up morale
        {
            addMorale(-20);
        }
    }

    private void checkLose()
    {
        if(morale < 0)
        {
            Debug.Log("Your crew kills you. You lose :(");
        }
    }

    private void startRandomEvent()
    {
        Debug.Log("Random Event!");
        if(currentEventIndex >= events.Count)
        {
            setButtonsInteractable(true);
            return;
        }
        dialogueDisplay.dialogueContainer = events[currentEventIndex];
        currentEventIndex++;
        dialogueDisplay.init();
        dialogueDisplay.gameObject.SetActive(true);
    }

    private DialogueContainer pickRandomEvent()
    {
        bool searching = true;
        while(searching)
        {
            int randomIndex = Random.Range(0, events.Count);
            if(events[randomIndex].alreadyEncountered == true)
            {
                continue;
            }
            events[randomIndex].alreadyEncountered = true;
            return events[randomIndex];
        }
        return null;
    }

    public void addMorale(int value)
    {
        Debug.Log("Morale changed: " + value);
        morale += value;
        if(morale > 100)
        {
            morale = 100;
        }
        updateResUI();
    }
    public void addRes(int value)
    {
        Debug.Log("Res changed: " + value);
        resources += value;
        if(resources > 100)
        {
            resources = 100;
        }
        updateResUI();
    }

    int getMorale()
    {
        return morale;
    }

    int getResources()
    {
        return resources;
    }

    void updateResUI()
    {
        resource_display.updateUI(morale, resources);
    }
}


