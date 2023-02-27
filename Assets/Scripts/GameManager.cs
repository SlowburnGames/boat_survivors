using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public int population = 3;
    public List<BaseHero> startingHeroes;
    public List<BaseEnemy> enemiesInCombat;

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

    public int combatMoraleReward;
    public int combatResReward;

    public bool intro_played = false;

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
    }

    private void Start()
    {
        loadRandomEvents();
        morale = 50;
        resource = 50;
        active_status_effects = new List<StatusEffect>();
        isDay = true;
        travel_distance = 0;
        population = 3;

        // Reset Health
        foreach (var hero in startingHeroes)
            hero.unitClass.health = hero.unitClass.maxHealth;
    }

    public void startCombat(List<string> enemyNames)
    {
        enemiesInCombat.Clear();

        foreach (var enemyName in enemyNames)
            enemiesInCombat.Add(UnitManager.Instance.GetEnemyByName(enemyName));
        
        SceneManager.LoadScene("Combat");
    }

    public void resetGame()
    {
        loadRandomEvents();
        morale = 50;
        resource = 50;
        active_status_effects = new List<StatusEffect>();
        isDay = true;
        travel_distance = 0;
        population = 3;
        Debug.Log("Morale " + morale);
        SceneManager.LoadScene("Travel");
    }

    public void exitGame()
    {
        Debug.Log("it's working");
        Application.Quit();
    }

    void loadRandomEvents()
    {
        currentEventIndex = 0;
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
        // Example for healing all heroes (for now before EVERY event)
        // healAllHeroes(2);
        // healHero(startingHeroes.First(), 666);
        
        
        // DEBUG ONLY START
        Debug.Log("TODO: NEXT LINE ONLY FOR DEBUG REASONS");
        startCombat(new List<string>{"Monster1"});
        // DEBUG ONLY END
        
        Debug.Log("Random Event!");
        if(currentEventIndex >= events.Count)
        {
            SceneManager.LoadScene("Win");
            return;
        }
        DialogueDisplay.instance.dialogueContainer = events[currentEventIndex];
        currentEventIndex++;
        DialogueDisplay.instance.init();
    }

    public void healAllHeroes(int amount)
    {
        foreach (var hero in startingHeroes)
            hero.unitClass.Heal(amount);
    }

    public void healHero(BaseHero hero, int amount)
    {
        hero.unitClass.Heal(amount);
    }

    public void addCombatRewards()
    {
        morale += combatMoraleReward;
        resource += combatResReward;
    }

    public void setCombatRewards(int morale, int res)
    {
        combatMoraleReward = morale;
        combatResReward = res;
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