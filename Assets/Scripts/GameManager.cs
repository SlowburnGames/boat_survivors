using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public int population = 3;
    public List<string> currentHeroes;
    public List<string> heroesInCombat;
    public List<string> enemiesInCombat;
    private int morale = 50;
    public int Morale
    {
        get{ return morale; }
        set{
            morale = value;
            TravelManager.instance.updateResUI();
        }
    }
    private int resource = 50;
    public int Resource
    {
        get{ return resource; }
        set{
            resource = value;
            TravelManager.instance.updateResUI();
        }
    }
    public List<DialogueContainer> events;
    public int currentEventIndex = 0;
    public bool isDay = true;
    public int travel_distance = 0;
    public List<StatusEffect> active_status_effects = new List<StatusEffect>();

    public void Awake()
    {
        DontDestroyOnLoad(this);
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        currentHeroes = new List<string>();
        currentHeroes.Add("Rouge");
        currentHeroes.Add("Fighter");
        currentHeroes.Add("Wizard");
    }

    private void Start()
    {
        currentHeroes = new List<string>();
        currentHeroes.Add("Rouge");
        currentHeroes.Add("Fighter");
        currentHeroes.Add("Wizard");

        loadRandomEvents();

        if(SceneManager.GetActiveScene().name != "Combat")
            SceneManager.LoadScene("Travel");
    }

    List<string> makeHeroList()
    {
        List<string> stringList = new List<string>();
        foreach (var hero in currentHeroes)
        {
        }
        return stringList;
    }

    public void startCombat(List<string> enemies)
    {
        heroesInCombat = currentHeroes;
        enemiesInCombat = enemies;
        SceneManager.LoadScene("Combat");
    }

    public void resetGame()
    {
        Start();
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

    public void addStatus(StatusEffect statusEffect)
    {
        active_status_effects.Add(statusEffect);
    }

    public void applyStatusEffects()
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

    public void consumeResources()
    {
        if(resource > 0)
        {
            //TODO: change depending on amount of people on the ship
            addMorale(population * -2);
        }
        else //if resources are empty use up morale
        {
            addMorale(20);
        }
    }

    public void checkLose()
    {
        if(morale < 0)
        {
            Debug.Log("Your crew kills you. You lose :(");
            SceneManager.LoadScene("GameOver");
        }
    }

    public void startRandomEvent()
    {
        Debug.Log("Random Event!");
        if(currentEventIndex >= events.Count)
        {
            TravelManager.instance.setButtonsInteractable(true);
            return;
        }
        DialogueDisplay.instance.dialogueContainer = events[currentEventIndex];
        currentEventIndex++;
        DialogueDisplay.instance.init();
    }

    public void addRes(int value)
    {
        resource += value;
        TravelManager.instance.updateResUI();
    }

    public void addMorale(int value)
    {
        morale += value;
        TravelManager.instance.updateResUI();
    }

}