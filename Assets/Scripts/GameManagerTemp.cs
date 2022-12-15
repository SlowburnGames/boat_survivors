using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagerTemp : MonoBehaviour
{
    public static GameManagerTemp Instance;
    public List<string> spawnableHeroes = new List<string> { "Fighter", "Rouge", "Wizard", "Fighter" };
    public List<string> spawnableEnemies = new List<string> { "Monster", "Zombie", "Monster", "Zombie" };

    public void Awake()
    {
        Instance = this;
    }
}