using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class UnitManager : MonoBehaviour
{
    public static UnitManager Instance;
    private List<ScriptableUnit> _units;

    public BaseHero selectedHero;
    
    private void Awake()
    {
        Instance = this;

        _units = Resources.LoadAll<ScriptableUnit>("Combat/Units").ToList();
        Debug.Log("Nr of units: " + _units.Count);
    }

    public void SpawnHeroes()
    {
        // TODO Select heroes to spwan from menue
        int heroCount = 3;

        for (int i = 0; i < heroCount; i++)
        {
            var randomPrefab = GetRandomUnit<BaseHero>(Faction.Hero);
            var spawnedHero = Instantiate(randomPrefab);
            spawnedHero.MovementRange = 6;
            var randomSpawnTile = WFCGenerator.Instance.GetHeroSpawnTile();

            randomSpawnTile.SetUnit(spawnedHero);
        }
        
        CombatManager.Instance.ChangeCombatState(CombatState.SpawnEnemies);
    }
    
    public void SpawnEnemies()
    {
        // TODO Not hardcode count
        int enemyCount = 3;

        for (int i = 0; i < enemyCount; i++)
        {
            var randomPrefab = GetRandomUnit<BaseEnemy>(Faction.Enemy);
            var spawnedEnemy = Instantiate(randomPrefab);
            var randomSpawnTile = WFCGenerator.Instance.GetEnemySpawnTile();

            randomSpawnTile.SetUnit(spawnedEnemy);
        }
        
        CombatManager.Instance.ChangeCombatState(CombatState.HeroesTurn);
    }
    

    private T GetRandomUnit<T>(Faction faction) where T : BaseUnit
    {
        //Debug.Log(_units.Where(u => u.Faction == faction).Count());
        
        return (T)_units.Where(u => u.Faction == faction).OrderBy(o => Random.value).First().UnitPrefab;
    }

    public void SetSelectedHero(BaseHero hero)
    {
        selectedHero = hero;
        MenuManager.Instance.ShowSelectedHero(hero);
    }
    
}
