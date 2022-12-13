using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public List<string> currentHeroes;

    public List<string> heroesInCombat;
    public List<string> enemiesInCombat;

    public void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(this);
    }

    private void Start()
    {
        currentHeroes = new List<string>();
        currentHeroes.Add("Rogue");
        currentHeroes.Add("Fighter");
        currentHeroes.Add("Wizard");
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

}