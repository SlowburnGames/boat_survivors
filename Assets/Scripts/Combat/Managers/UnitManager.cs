using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class UnitManager : MonoBehaviour
{
    public static UnitManager Instance;
    private List<ScriptableUnit> _enemyUnits;
    private List<ScriptableUnit> _heroUnits;

    public BaseHero selectedHero;
    private static List<string> _availableHeroes;

    [SerializeField] private GameObject _attackRangeIndicator;

    private void Awake()
    {
        Instance = this;

        _enemyUnits = Resources.LoadAll<ScriptableUnit>("Combat/Units/Enemies").ToList();
        _heroUnits = Resources.LoadAll<ScriptableUnit>("Combat/Units/Heroes").ToList();
        Debug.Log("Nr of different Enemies: " + _enemyUnits.Count);
        Debug.Log("Nr of different Heroes : " + _heroUnits.Count);
    }

    public void SpawnHeroes()
    {
        // TODO Select heroes to spwan from menue
        foreach (var hero in _heroUnits)
        {
            var spawnedHero = Instantiate(hero.UnitPrefab);
            var randomSpawnTile = WFCGenerator.Instance.GetHeroSpawnTile(); // TODO Let user select spwan

            SetUnit(spawnedHero, randomSpawnTile);
        }
        
        CombatManager.Instance.ChangeCombatState(CombatState.HeroesTurn);
    }

    public void SpawnSelectedHero(Tile spawnTile)
    {
        if (_availableHeroes.Count > 0)
        {
            var heroPrefab = GetHeroByName(_availableHeroes.First());
            var spawnedHero = Instantiate(heroPrefab);
            
            SetUnit(spawnedHero, spawnTile);
            _availableHeroes.RemoveAt(0);
            MenuManager.Instance.UpdateAvailableHeroes(_availableHeroes);
        }

        if (_availableHeroes.Count <= 0)
        {
            CombatManager.Instance.ChangeCombatState(CombatState.HeroesTurn);
            MenuManager.Instance.DisableAvailableHeroes();
        }
        
    }
    
    public void SpawnEnemies()
    {
        // TODO Get type and number of enemies from outside
        string[] enemySequence = { "Monster", "Zombie", "Monster", "Zombie" };

        foreach (var enemyName in enemySequence)
        {
            var enemyPrefab = GetEnemyByName(enemyName);
            var spawnedEnemy = Instantiate(enemyPrefab);
            var randomSpawnTile = WFCGenerator.Instance.GetEnemySpawnTile();

            SetUnit(spawnedEnemy, randomSpawnTile);
        }
        
        CombatManager.Instance.ChangeCombatState(CombatState.SpawnHeroes);
    }

    public void HeroesTurn(Tile tile, BaseUnit tileUnit, bool isWalkable)
    {
        // Attacking enemy branch
        if (tileUnit != null)
        {
            // First click -> select the hero and range indicator
            if (tileUnit.Faction == Faction.Hero && selectedHero == null)
            {
                SetSelectedHero((BaseHero) tileUnit);
                
                // TODO 1: wenn man hero und danach ANDEREN hero selected muss der indicator beim ersten gelöscht werden
                // TODO 2: wenn man hero und danach SELBEN hero selected darf nicht nochmal in dies if rein
                ToggleAttackRangeIndicator((BaseHero)tileUnit, true);
            }
            // Second click on same hero (nothing should happen)
            else if (tileUnit.Faction == Faction.Hero && selectedHero == (BaseHero)tileUnit)
            {
                Debug.Log("Same hero selected");
            }
            // Second click on either another hero -> (hero1 gets unselected and hero2 gets selected)
            else if (tileUnit.Faction == Faction.Hero && selectedHero != null)
            {
                Debug.Log("Another hero selected");
                ToggleAttackRangeIndicator(selectedHero, false);
                SetSelectedHero((BaseHero)tileUnit);
                ToggleAttackRangeIndicator((BaseHero)tileUnit, true);
            }
            else
            {
                // When we next click on an enemy -> Attack it
                if (selectedHero != null && tileUnit.Faction == Faction.Enemy)
                {
                    var enemy = (BaseEnemy) tileUnit;
                    bool canAttack = false;
                    
                    // Check if attack possible
                    canAttack = CheckAttackPossible(selectedHero, enemy);
                    
                    // Do the attack (dmg + unselecting unit)
                    if (canAttack)
                    {
                        enemy.Attack(selectedHero.AttackDamage);
                        Debug.Log("Damaged " + enemy.name + " by " + selectedHero.AttackDamage);
                        
                        // Check if attacked unit dies
                        CheckAttackedUnit(enemy);
                        
                        // End turn
                        ToggleAttackRangeIndicator(selectedHero, false);
                        SetSelectedHero(null);
                        CombatManager.Instance.ChangeCombatState(CombatState.EnemiesTurn);
                    }
                    
                }
            }
        }
        // Moving branch
        else if (tileUnit == null)
        {
            // When we next click on an empty tile -> Move Hero to this tile
            if (selectedHero != null && isWalkable && tile.tileUnit == null)
            {
                ToggleAttackRangeIndicator(selectedHero, false);
                SetUnit(selectedHero, tile);
                SetSelectedHero(null);
                CombatManager.Instance.ChangeCombatState(CombatState.EnemiesTurn);
            }
        }
    }
    
    public void EnemiesTurn()
    {
        
        Debug.Log("Enemy turn!");
        CombatManager.Instance.ChangeCombatState(CombatState.HeroesTurn);
    }

    private BaseEnemy GetEnemyByName(string eName)
    {
        return (BaseEnemy)_enemyUnits.Where(u => u.name == eName).First().UnitPrefab;
    }
    
    public BaseHero GetHeroByName(string hName)
    {
        return (BaseHero)_heroUnits.Where(u => u.name == hName).First().UnitPrefab;
    }

    public void SetSelectedHero(BaseHero hero)
    {
        selectedHero = hero;
        MenuManager.Instance.ShowSelectedHero(hero);
    }

    public void ToggleAttackRangeIndicator(BaseHero hero, bool show)
    {
        List<Tile> indicatorPositions;
        if (hero.IsRanged)
            indicatorPositions = CalcRangedAttackIndicator(hero.OccupiedTile, hero.AttackRange);
        else
            indicatorPositions = CalcMeleeAttackIndicator(hero.OccupiedTile);
        
        if (show)
            SetAttackRangeIndicator(indicatorPositions);
        else
            UnsetAttackRangeIndicator(indicatorPositions);
    }

    private List<Tile> CalcMeleeAttackIndicator(Tile heroTile)
    {
        int xStart = (int)heroTile.tilePosition.x;
        int yStart= (int)heroTile.tilePosition.y;
        List<Tile> indicaterPositions = new List<Tile>();
        int maxX = (int)WFCGenerator.Instance.gridSize.x;
        int maxY = (int)WFCGenerator.Instance.gridSize.y;

        for (int x = xStart-1; x <= xStart+1; x++)
            for (int y = yStart-1; y <= yStart+1; y++)
            {
                if ((x == xStart && y == yStart) || x >= maxX || x < 0 || y >= maxY || y < 0)
                    continue;
                
                indicaterPositions.Add(WFCGenerator.Instance._tiles[x][y]);
            }

        return indicaterPositions;
    }

    private List<Tile> CalcRangedAttackIndicator(Tile heroTile, int range)
    {
        return null;
    }

    private void SetAttackRangeIndicator(List<Tile> tilePositions) 
    {
        foreach (var tilePos in tilePositions)
            Instantiate(_attackRangeIndicator, tilePos.transform);
    }

    public void UnsetAttackRangeIndicator(List<Tile> tilePositions)
    {
        foreach (var tilePos in tilePositions)
            if(tilePos.transform.childCount != 0)
                Destroy(tilePos.transform.GetChild(0).gameObject);

    }
    
    public void SetUnit(BaseUnit unit, Tile tile)
    {
        if (unit.OccupiedTile != null)
            unit.OccupiedTile.tileUnit = null;
        unit.transform.position = tile.transform.position + Vector3.up;
        unit.transform.LookAt(FindObjectOfType<Camera>().transform.position, Vector3.up);
        tile.tileUnit = unit;
        unit.OccupiedTile = tile;
    }

    private bool CheckAttackPossible(BaseUnit attackUnit, BaseUnit defendUnit)
    {
        var attackPos = attackUnit.OccupiedTile.tilePosition;
        var defendPos = defendUnit.OccupiedTile.tilePosition;
        
        if (attackUnit.IsRanged)
        {
            // Calc with 4-neighborhood
            var subtract = attackPos - defendPos;
            int distance;
            // Check if they are on same row or col
            if (subtract.x == 0) distance = (int)subtract.y;
            else if (subtract.y == 0) distance = (int)subtract.x;
            else return false;  // Not on same row or col

            return distance <= attackUnit.AttackRange;
        }
        else
        {
            // Calc with (single) 8-neighborhood if needed
            var distance = Vector2.Distance(attackPos, defendPos);
            if ( (int)distance <= attackUnit.AttackRange)
                return true;
            return false;
        }
    }
    
    private void CheckAttackedUnit(BaseUnit aUnit)
    {
        if (aUnit.Health <= 0)
            Destroy(aUnit.gameObject);
    }

    public void SetSpawnableHeroes(List<string> heroes)
    {
        _availableHeroes = heroes;
    }

    public BaseHero GetNextHero()
    {
        return GetHeroByName(_availableHeroes[0]);
    }
    
    
}
