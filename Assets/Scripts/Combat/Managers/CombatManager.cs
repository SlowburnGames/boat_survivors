using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class CombatManager : MonoBehaviour
{
    public static CombatManager Instance;
    public CombatState combatState;
    public Queue<BaseUnit> _turnQueue = new Queue<BaseUnit>();
    public List<BaseUnit> _spawnedUnitList = new List<BaseUnit>();
    
    // DEBUG ONLY
    public List<BaseHero> _defaultHeroes = new List<BaseHero>();
    public List<BaseEnemy> _defaultEnemies = new List<BaseEnemy>();
    // DEBUG ONLY

    private int turnCounter = 0;
    private int currentTurn = 0;
    
    public void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        ChangeCombatState(CombatState.SetHeroesAndEnemies);
    }
    
    public void ChangeCombatState(CombatState newState)
    {
        combatState = newState;
        switch (newState)
        {
            case CombatState.SetHeroesAndEnemies:
                
                // COMBAT SCREEN ONLY START
                var START_IN_COMBAT_SCREEN = true;
                if (START_IN_COMBAT_SCREEN)
                {
                    GameManager.Instance.startingHeroes = _defaultHeroes;
                    GameManager.Instance.enemiesInCombat = _defaultEnemies;
                }
                
                UnitManager.Instance.SetUnitIDs(GameManager.Instance.startingHeroes, GameManager.Instance.enemiesInCombat);
                
                UnitManager.Instance.SetSpawnableHeroes(GameManager.Instance.startingHeroes);
                UnitManager.Instance.SetSpawnableEnemies(GameManager.Instance.enemiesInCombat);
                
                ChangeCombatState(CombatState.GenerateGrid);
                break;
            case CombatState.GenerateGrid:
                WFCGenerator.Instance.runWFC();
                break;
            case CombatState.SpawnEnemies:
                UnitManager.Instance.SpawnEnemies();
                break;
            case CombatState.SpawnHeroes:
                // Logic in Tile.OnMouseDown and UnitManager.SpawnSelectedHero
                Debug.Log("Spawn Heroes by clicking on tile!");
                MenuManager.Instance.ShowAvailableHeroes(GameManager.Instance.startingHeroes);
                break;
            case CombatState.SetTurnOrder:
                SetTurnOrder();
                break;
            case CombatState.UnitTurn:
                UnitManager.Instance.checkCombatOver();
                BaseUnit nextUnit = _turnQueue.Dequeue();
                _turnQueue.Enqueue(nextUnit);

                currentTurn++;
                if(currentTurn == turnCounter)
                {
                    currentTurn = 0;
                    updateTimers();
                }

                if (nextUnit.Faction == Faction.Hero)
                {
                    var hero = (BaseHero) nextUnit;
                    Debug.Log("Hero " + hero.UnitName + " turn!");
                    
                    //select the hero and range indicator
                    UnitManager.Instance.SetSelectedHero(hero);
                    UnitManager.Instance.ToggleAttackRangeIndicator(hero, true);
                    combatState = CombatState.HeroTurn;
                    MenuManager.Instance.toggleButtons(true);
                }
                else if (nextUnit.Faction == Faction.Enemy)
                {
                    var enemy = (BaseEnemy) nextUnit;
                    Debug.Log("Enemy " + enemy.UnitName + " turn!");
                    
                    UnitManager.Instance.EnemiesTurn(enemy);
                }
                break;
            case CombatState.CombatEnd:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(newState), newState, null);
        }
        
    }

    private void SetTurnOrder()
    {
        _spawnedUnitList.Shuffle();

        foreach (var unit in _spawnedUnitList)
            _turnQueue.Enqueue(unit);

        turnCounter = _turnQueue.Count;
        
        ChangeCombatState(CombatState.UnitTurn);
    }
    
    public void endPlayerTurn()
    {
        UnitManager.Instance.selectedHero.tilesWalked = 0;
        //UnitManager.Instance.selectedHero.usedAction = false;
        UnitManager.Instance.ToggleAttackRangeIndicator(UnitManager.Instance.selectedHero, false);
        UnitManager.Instance.SetSelectedHero(null);
        ChangeCombatState(CombatState.UnitTurn);
    }

    public void updateTimers()
    {
        foreach (var unit in _turnQueue)
        {
            unit.updateCooldowns();
        }
        
    }
    
    
}

public enum CombatState
{
    SetHeroesAndEnemies = 0,
    GenerateGrid = 1,
    SpawnEnemies = 2,
    SpawnHeroes = 3,
    SetTurnOrder = 4,
    UnitTurn = 5,
    HeroTurn = 6,
    EnemyTurn = 7,
    CombatEnd = 8
}
