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
                UnitManager.Instance.SetSpawnableHeroes(GameManager.Instance.currentHeroes);
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
                MenuManager.Instance.ShowAvailableHeroes(GameManager.Instance.currentHeroes);
                break;
            case CombatState.SetTurnOrder:
                SetTurnOrder();
                break;
            case CombatState.UnitTurn:

                UnitManager.Instance.checkCombatOver();
                BaseUnit nextUnit = _turnQueue.Dequeue();
                _turnQueue.Enqueue(nextUnit);

                if (nextUnit.Faction == Faction.Hero)
                {
                    var hero = (BaseHero) nextUnit;
                    Debug.Log("Hero " + hero.name + " turn!");
                    
                    //select the hero and range indicator
                    UnitManager.Instance.SetSelectedHero(hero);
                    UnitManager.Instance.ToggleAttackRangeIndicator(hero, true);
                    combatState = CombatState.HeroTurn;
                    MenuManager.Instance.toggleButtons(true);
                }
                else if (nextUnit.Faction == Faction.Enemy)
                {
                    var enemy = (BaseEnemy) nextUnit;
                    Debug.Log("Enemy " + enemy.name + " turn!");
                    
                    // TODO implement enemy turn
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
        
        ChangeCombatState(CombatState.UnitTurn);
    }
    
    public void endPlayerTurn()
    {
        UnitManager.Instance.selectedHero.tilesWalkedThisTurn = 0;
        UnitManager.Instance.selectedHero.usedAction = false;
        UnitManager.Instance.ToggleAttackRangeIndicator(UnitManager.Instance.selectedHero, false);
        UnitManager.Instance.SetSelectedHero(null);
        ChangeCombatState(CombatState.UnitTurn);
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
