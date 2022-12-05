using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class TravelManager : MonoBehaviour
{

    [SerializeField]private int morale = 50;
    [SerializeField]private int resources = 50;
    [SerializeField]public int travel_distance = 0;

    [SerializeField]public RessourceDisplay resource_display;

    [SerializeField]private Button[] buttons;
    [SerializeField]private List<Event> events;

    // Start is called before the first frame update
    void Start()
    {
        ReadEvents();
        updateResUI();
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

    public void sail()
    {
        //set animations for sailing
        //wait for some time 2 seconds or so
        foreach (Button button in buttons)
        {
            button.interactable = false;
        }

        Debug.Log("Starting Sailing");

        startRandomEvent();

    }

    private void startRandomEvent()
    {

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


