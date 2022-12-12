using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class TravelManager : MonoBehaviour
{

    [SerializeField]private int morale = 50;
    [SerializeField]private int resources = 50;
    [SerializeField]public RessourceDisplay resource_display;
    [SerializeField]private Button[] buttons;
    [SerializeField]private List<Event> events;

    [SerializeField]public Sprite sun_sprite;
    [SerializeField]public Sprite moon_sprite;


    private bool isDay = true;
    public int travel_distance = 0;
    public int population = 3;

    private Animator boat_animator;
    private GameObject time_display;
    private List<StatusEffect> active_status_effects = new List<StatusEffect>();


    // Start is called before the first frame update
    void Start()
    {
        setupCamera();
        updateResUI();
        Transform canvas = transform.Find("Canvas");
        Transform time_display_transform = canvas.Find("TimeDisplay");
        time_display = time_display_transform.gameObject;
        Transform boat = canvas.Find("Boat");
        boat_animator = boat.GetComponent<Animator>();

        GenericStatus test = new GenericStatus(this, +5, +5, 3);
        active_status_effects.Add(test);
    }


    void setupCamera()
    {
        Camera.main.orthographicSize = 100 * Screen.height / Screen.width * 0.5f;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void ReadEvents()
    {
        string[] lines = System.IO.File.ReadAllLines("Assets/CSV/Events.csv");
        string[] Columns= lines[0].Split(';');

        for (int i=1; i<=lines.Length-1; i++)
        {
            if(lines[i].Length > 0)
            {
                string[] cols = lines[i].Split(';');
                Event new_event = new Event(cols);
                events.Add(new_event);
            }
        }
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
        setButtonsInteractable(true);

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

    private void setButtonsInteractable(bool interactable)
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
    }

    public void addMorale(int value)
    {
        morale += value;
        updateResUI();
    }
    public void addRes(int value)
    {
        resources += value;
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


