using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class CombatManager : MonoBehaviour
{
    public static CombatManager Instance;
    [FormerlySerializedAs("GameState")] public CombatState combatState;

    public void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        ChangeCombatState(CombatState.GenerateGrid);
    }

    public void ChangeCombatState(CombatState newState)
    {
        combatState = newState;
        switch (newState)
        {
            case CombatState.GenerateGrid:
                WFCGenerator.Instance.runWFC();
                break;
            case CombatState.SpawnHeroes:
                UnitManager.Instance.SpawnHeroes();
                break;
            case CombatState.SpawnEnemies:
                UnitManager.Instance.SpawnEnemies();
                break;
            case CombatState.HeroesTurn:
                break;
            case CombatState.EnemiesTurn:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(newState), newState, null);
        }
    }
}

public enum CombatState
{
    GenerateGrid = 0,
    SpawnHeroes = 1,
    SpawnEnemies = 2,
    HeroesTurn = 3,
    EnemiesTurn = 4
}
