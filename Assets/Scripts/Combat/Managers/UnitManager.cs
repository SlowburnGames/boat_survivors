using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using Debug = UnityEngine.Debug;

public class UnitManager : MonoBehaviour
{
    public static UnitManager Instance;
    private List<ScriptableUnit> _enemyUnits;
    private List<ScriptableUnit> _heroUnits;
    private List<ScriptableUnit> _allUnits;

    public BaseHero selectedHero;
    private static List<string> _availableHeroes;
    private static List<string> _availableEnemies;

    [SerializeField] private GameObject _attackRangeIndicator;

    private void Awake()
    {
        Instance = this;

        _enemyUnits = Resources.LoadAll<ScriptableUnit>("Combat/Units/Enemies").ToList();
        _heroUnits = Resources.LoadAll<ScriptableUnit>("Combat/Units/Heroes").ToList();
        _allUnits = Resources.LoadAll<ScriptableUnit>("Combat/Units").ToList();
        Debug.Log("Nr of different Enemies: " + _enemyUnits.Count);
        Debug.Log("Nr of different Heroes : " + _heroUnits.Count);
    }

    public void SpawnSelectedHero(Tile spawnTile)
    {
        if (_availableHeroes.Count > 0)
        {
            var heroPrefab = GetHeroByName(_availableHeroes.First());
            var spawnedHero = Instantiate(heroPrefab);
            
            SetUnit(spawnedHero, spawnTile);
            CombatManager.Instance._spawnedUnitList.Add(spawnedHero);
            _availableHeroes.RemoveAt(0);
            MenuManager.Instance.UpdateAvailableHeroes(_availableHeroes);
        }

        if (_availableHeroes.Count <= 0)
        {
            CombatManager.Instance.ChangeCombatState(CombatState.SetTurnOrder);
            MenuManager.Instance.DisableAvailableHeroes();
        }
        
    }
    
    public void SpawnEnemies()
    {
        foreach (var enemyName in _availableEnemies)
        {
            var enemyPrefab = GetEnemyByName(enemyName);
            var spawnedEnemy = Instantiate(enemyPrefab);
            var randomSpawnTile = WFCGenerator.Instance.GetEnemySpawnTile();

            SetUnit(spawnedEnemy, randomSpawnTile);
            CombatManager.Instance._spawnedUnitList.Add(spawnedEnemy);
        }
        
        CombatManager.Instance.ChangeCombatState(CombatState.SpawnHeroes);
    }

    public void HeroesTurn(Tile tile, BaseUnit tileUnit, bool isWalkable)
    {
        // Attacking enemy branch
        if (tileUnit != null)
        {
            // Click on same hero or another hero (nothing should happen)
            if (tileUnit.Faction == Faction.Hero && (selectedHero == (BaseHero)tileUnit || selectedHero != null))
            {
                // Debug.Log("Same hero selected");
            }
            // Click on an enemy -> Attack it (if possible, else nothing should happen)
            else
            {
                if (selectedHero != null && tileUnit.Faction == Faction.Enemy)
                {
                    var enemy = (BaseEnemy) tileUnit;
                    
                    // Check if attack possible
                    bool canAttack = CheckAttackPossible(selectedHero, enemy);
                    
                    // Do the attack (dmg + unselecting unit)
                    if (canAttack)
                    {
                        enemy.Attack(selectedHero.AttackDamage);
                        // Debug.Log("Damaged " + enemy.name + " by " + selectedHero.AttackDamage);
                        
                        // Check if attacked unit dies
                        CheckAttackedUnit(enemy);
                        
                        // End turn
                        ToggleAttackRangeIndicator(selectedHero, false);
                        SetSelectedHero(null);
                        CombatManager.Instance.ChangeCombatState(CombatState.UnitTurn);
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
                if(tile.calculatedDistance(selectedHero.OccupiedTile.gameObject) <= selectedHero.MoveDistance)
                {
                    ToggleAttackRangeIndicator(selectedHero, false);
                    Pathfinding.Instance.FindPath(selectedHero.OccupiedTile, tile);
                    SetUnit(selectedHero, tile);
                    SetSelectedHero(null);
                    CombatManager.Instance.ChangeCombatState(CombatState.UnitTurn);
                    
                }
                else
                {
                    Debug.Log("cant move this far (distance: " + tile.calculatedDistance(selectedHero.OccupiedTile.gameObject) + ")");
                }
            }
            else
            {
                Debug.Log("cant move there");
            }
        }
    }
    
    
    public void EnemiesTurn(BaseEnemy enemy)
    {
        Debug.Log("Enemy turn!");

        var target = findNearestHero(enemy);

        BaseUnit targetedHero = target.Item1;
        int distance = target.Item2;
        List<Tile> path = target.Item3;

        //if enemy is reachable with melee attack
        if(distance <= enemy.MoveDistance)
        {
            if(distance > 1)
            {
                SetUnit(enemy, path[path.Count - 2]);
            }
            targetedHero.Attack(enemy.AttackDamage);
        }
        else //hero is not reachable
        {
            SetUnit(enemy, path[enemy.MoveDistance - 1]);
        }

        CombatManager.Instance.ChangeCombatState(CombatState.UnitTurn);
    }

    Tuple<BaseUnit, int, List<Tile>> findNearestHero(BaseEnemy enemy)
    {
        List<BaseUnit> allHeroes = CombatManager.Instance._turnQueue.ToList().FindAll(unit => unit.Faction == Faction.Hero);

        BaseUnit nearestHero = null;
        int minDistance = Int32.MaxValue;
        List<Tile> best_path = new List<Tile>();

        foreach (var hero in allHeroes)
        {
            List<Tile> path = Pathfinding.Instance.FindPath(enemy.OccupiedTile, hero.OccupiedTile);
            if(minDistance > path.Count)
            {
                minDistance = path.Count;
                nearestHero = hero;
                best_path = path;
            }
        }

        return Tuple.Create(nearestHero, minDistance, best_path);

    }
    private BaseEnemy GetEnemyByName(string eName)
    {
        return (BaseEnemy)_enemyUnits.Where(u => u.name == eName).First().UnitPrefab;
    }
    
    public BaseHero GetHeroByName(string hName)
    {
        return (BaseHero)_heroUnits.Where(u => u.name == hName).First().UnitPrefab;
    }
    
    public BaseUnit GetUnitByName(string uName)
    {
        return (BaseUnit)_allUnits.Where(u => u.name == uName).First().UnitPrefab;
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
        int xStart = (int)heroTile.tilePosition.x;
        int yStart= (int)heroTile.tilePosition.y;
        List<Tile> indicaterPositions = new List<Tile>();
        int maxX = (int)WFCGenerator.Instance.gridSize.x;
        int maxY = (int)WFCGenerator.Instance.gridSize.y;

        for (int x = xStart - range; x <= xStart + range; x++)
        {
            if (x == xStart || x >= maxX || x < 0 )
                continue;

            indicaterPositions.Add(WFCGenerator.Instance._tiles[x][yStart]);
        }
        for (int y = yStart-range; y <= yStart+range; y++)
        {
            if (y == yStart ||  y >= maxY || y < 0)
                continue;
                
            indicaterPositions.Add(WFCGenerator.Instance._tiles[xStart][y]);
        }

        return indicaterPositions;
    }

    private void SetAttackRangeIndicator(List<Tile> tilePositions) 
    {
        foreach (var tilePos in tilePositions)
        {
            tilePos.transform.GetChild(1).GetComponent<SpriteRenderer>().color = Color.blue;
            tilePos.transform.GetChild(1).gameObject.gameObject.SetActive(true);
            tilePos.transform.GetChild(1).localPosition = new Vector3(0, 0.6f, 0);
            // Instantiate(_attackRangeIndicator, tilePos.transform);
        }
    }

    public void UnsetAttackRangeIndicator(List<Tile> tilePositions)
    {
        foreach (var tilePos in tilePositions)
            if (tilePos.transform.childCount != 0)
            {
                tilePos.transform.GetChild(1).gameObject.SetActive(false);
                // Destroy(tilePos.transform.GetChild(0).gameObject);
            }

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
        {
            CombatManager.Instance._turnQueue =
                new Queue<BaseUnit>(CombatManager.Instance._turnQueue.Where(x => x != aUnit));
            Destroy(aUnit.gameObject);
        }
    }

    public BaseHero GetNextHero()
    {
        return GetHeroByName(_availableHeroes[0]);
    }
    
    public void SetSpawnableHeroes(List<string> heroes)
    {
        _availableHeroes = new List<string>(heroes);
    }
    public void SetSpawnableEnemies(List<string> enemies)
    {
        _availableEnemies = new List<string>(enemies);
    }

    public void checkCombatOver()
    {
        if (CombatManager.Instance._turnQueue.ToList().FindAll(x => x.Faction == Faction.Enemy).Count == 0)
        {
            SceneManager.LoadScene("Travel");
        }
        else if (CombatManager.Instance._turnQueue.ToList().FindAll(x => x.Faction == Faction.Hero).Count == 0)
        {
            SceneManager.LoadScene("Travel");
        }
    }
    
    
}
