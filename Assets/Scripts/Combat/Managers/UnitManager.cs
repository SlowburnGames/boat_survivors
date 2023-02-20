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
    private List<BaseHero> _availableHeroes;
    private int _previewHeroCount = 0;
    // private List<BaseHero> _spawnableHeroes;
    private static List<BaseEnemy> _availableEnemies;
    
    private static int unitIDCount = 0;

    [SerializeField] private GameObject _attackRangeIndicator;

    private void Awake()
    {
        Instance = this;

        _enemyUnits = Resources.LoadAll<ScriptableUnit>("Combat/Units/Enemies").ToList();
        _heroUnits = Resources.LoadAll<ScriptableUnit>("Combat/Units/Heroes").ToList();
        _allUnits = Resources.LoadAll<ScriptableUnit>("Combat/Units").ToList();
        Debug.Log("Nr of different Enemy Types: " + _enemyUnits.Count);
        Debug.Log("Nr of different Hero Types : " + _heroUnits.Count);
    }

    public void SpawnSelectedHero(Tile spawnTile)
    {

        if (_previewHeroCount < _availableHeroes.Count)
        {
            var heroPrefab = getNextHero();
            var spawnedHero = Instantiate(heroPrefab);
            SetUnit(spawnedHero, spawnTile);
            _previewHeroCount++;
            CombatManager.Instance._spawnedUnitList.Add(spawnedHero);
            
            // TODO -> look UpdateAvailableHeroes method
            MenuManager.Instance.UpdateAvailableHeroes(
                _availableHeroes.GetRange(_previewHeroCount, _availableHeroes.Count-_previewHeroCount));
        }
        
        
        if (_previewHeroCount >= _availableHeroes.Count)
        {
            CombatManager.Instance.ChangeCombatState(CombatState.SetTurnOrder);
            MenuManager.Instance.DisableAvailableHeroes();
            _previewHeroCount = 0;
        }
    }

    public BaseHero getNextHero()
    {
        return _availableHeroes[_previewHeroCount];
    }
    
    public void SpawnEnemies()
    {
        
        foreach (var enemy in _availableEnemies)
        {
            var spawnedEnemy = Instantiate(enemy);
            var randomSpawnTile = WFCGenerator.Instance.GetEnemySpawnTile();
        
            SetUnit(spawnedEnemy, randomSpawnTile);
            CombatManager.Instance._spawnedUnitList.Add(spawnedEnemy);
        }
        CombatManager.Instance.ChangeCombatState(CombatState.SpawnHeroes);
    }

    public void useHeroAbility()
    {
        //if(!selectedHero.usedAction)
        {
            selectedHero.SpecialMove(null);
            //selectedHero.usedAction = true;
            ToggleAttackRangeIndicator(selectedHero, false);
            //if(selectedHero.usedAction && selectedHero.tilesWalked == selectedHero.MoveDistance)
            //{
            //    //CombatManager.Instance.endPlayerTurn();
            //}
        }
    }

    public void HeroesTurn(Tile tile, BaseUnit tileUnit, bool isWalkable)
    {   
        if (tileUnit == selectedHero && /*!selectedHero.usedAction &&*/ selectedHero.standAction)
        {
            if(selectedHero.AttacksMade - selectedHero.MaxAttacks > 0)
            {
                selectedHero.SpecialMove(null);
                //selectedHero.usedAction = true;
                ToggleAttackRangeIndicator(selectedHero, false);
            }
            
        }
        // Attacking enemy branch
        else if (tileUnit != null)
        {
            // Click on same hero or another hero (nothing should happen)
            if (tileUnit.Faction == Faction.Hero && (selectedHero == (BaseHero)tileUnit || selectedHero != null))
            {
                // Debug.Log("Same hero selected");
            }
            // Click on an enemy -> Attack it (if possible, else nothing should happen)
            else
            {
                if (selectedHero != null && tileUnit.Faction == Faction.Enemy /*&& !selectedHero.usedAction*/)
                {
                    // check how many attacks left
                    if (selectedHero.AttacksMade > 0)
                    {
                        var enemy = (BaseEnemy)tileUnit;

                        // Check if attack possible
                        bool canAttack = CheckAttackPossible(selectedHero, enemy);

                        // Do the attack (dmg + unselecting unit)
                        if (canAttack)
                        {
                            selectedHero.AttackTarget(enemy);
                            selectedHero.AttacksMade--;
                            MenuManager.Instance.updateAttacks(selectedHero);

                            // Debug.Log("Damaged " + enemy.name + " by " + selectedHero.AttackDamage);

                            // Check if attacked unit dies
                            CheckAttackedUnit(enemy);
                            //selectedHero.usedAction = true;

                            // End turn
                            ToggleAttackRangeIndicator(selectedHero, false);
                            //SetSelectedHero(null);
                            //CombatManager.Instance.ChangeCombatState(CombatState.HeroTurn);
                        }
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
                var path = Pathfinding.Instance.FindPath(selectedHero.OccupiedTile, tile);
                //print("[Unit] start: " + selectedHero.OccupiedTile + ", end: " + tile);
                if (path.Count <= selectedHero.MoveDistance - selectedHero.tilesWalked)
                {
                    ToggleAttackRangeIndicator(selectedHero, false);
                    SetUnit(selectedHero, tile);
                    // StartCoroutine(MoveUnit(selectedHero, path, tile));
                    selectedHero.tilesWalked += path.Count;
                    //if(!selectedHero.usedAction)
                    //{
                    //    ToggleAttackRangeIndicator(selectedHero,true);
                    //}
                    MenuManager.Instance.ShowSelectedHero(selectedHero);
                    //selectedHero.tilesWalkedThisTurn = 0;
                    //SetSelectedHero(null);
                    //CombatManager.Instance.ChangeCombatState(CombatState.UnitTurn);
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

        //if(selectedHero.usedAction && selectedHero.tilesWalked == selectedHero.MoveDistance)
        //{
        //    //CombatManager.Instance.endPlayerTurn();
        //}
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
                //StartCoroutine(MoveUnit(enemy, path, path[path.Count - 2]));
            }
            enemy.AttackTarget(targetedHero);
            CheckAttackedUnit(targetedHero);
        }
        else //hero is not reachable
        {
            if (!heroesAlive()) // When all heroes are dead -> Game over
                return;
            SetUnit(enemy, path[enemy.MoveDistance - 1]);
            //StartCoroutine(MoveUnit(enemy, path, path[enemy.MoveDistance - 1]));
        }

        findInvisible(enemy.OccupiedTile);
        checkCombatOver();
        CombatManager.Instance.ChangeCombatState(CombatState.UnitTurn);
    }

    void findInvisible(Tile middleTile)
    {
        List<Tile> neighbours = WFCGenerator.Instance.getNeighbours(middleTile);

        foreach (var tile in neighbours)
        {

            if(tile.tileUnit != null && tile.tileUnit.Faction == Faction.Hero && tile.tileUnit.invisible)
            {
                tile.tileUnit.invisible = false;
            }
        }
    }

    Tuple<BaseUnit, int, List<Tile>> findNearestHero(BaseEnemy enemy)
    {
        List<BaseUnit> allHeroes = CombatManager.Instance._turnQueue.ToList().FindAll(unit => unit.Faction == Faction.Hero);

        BaseUnit nearestHero = null;
        int minDistance = Int32.MaxValue;
        List<Tile> best_path = new List<Tile>();

        foreach (var hero in allHeroes)
        {
            if(hero.invisible)
                continue;

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
    public BaseEnemy GetEnemyByName(string eName)
    {
        return (BaseEnemy)_enemyUnits.Where(u => u.name == eName).First().UnitPrefab;
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
        MenuManager.Instance.UpdateHealthBar(unit);
    }

    public IEnumerator MoveUnit(BaseUnit unit, List<Tile> path, Tile tile)
    {
        float time = 0.5f;
        Vector3[] waypoints = new Vector3[path.Count];
        for (int i = 0; i < path.Count; i++)
        {
            waypoints[i] = path[i].transform.position + Vector3.up;
        }
        unit.transform.GetComponent<MovementManager>().moveUnit(waypoints, time);
        yield return new WaitForSeconds(time);
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
            if(aUnit.Faction == Faction.Hero)
            {
                
                // int index = _availableHeroes.Find((BaseHero)aUnit);
                int index = _availableHeroes.FindIndex(a => a.unitID == aUnit.unitID);
                _availableHeroes.RemoveAt(index);
                
                GameManager.Instance.combatMoraleReward -= 10;
                GameManager.Instance.combatResReward -= 10;
            }
            Destroy(aUnit.gameObject);
            MenuManager.Instance.updateTurnOrderDisplay();
            checkCombatOver();
        }
    }

    public void SetUnitIDs(List<BaseHero> heroes, List<BaseEnemy> enemies)
    {
        foreach (var hero in heroes)
        {
            hero.unitID = unitIDCount;
            unitIDCount++;
        }
        foreach (var enemy in enemies)
        {
            enemy.unitID = unitIDCount;
            unitIDCount++;
        }
    }

    public void SetSpawnableHeroes(List<BaseHero> heroes)
    {
        _availableHeroes = new List<BaseHero>(heroes);
        // _spawnableHeroes = new List<BaseHero>(heroes);
    }
    
    public void SetSpawnableEnemies(List<BaseEnemy> enemies)
    {
        _availableEnemies = new List<BaseEnemy>(enemies);
    }

    public bool heroesAlive()
    {
        return CombatManager.Instance._turnQueue.ToList().FindAll(x => x.Faction == Faction.Hero).Count != 0;
    }
    public void checkCombatOver()
    {
        if (CombatManager.Instance._turnQueue.ToList().FindAll(x => x.Faction == Faction.Enemy).Count == 0)
        {
            GameManager.Instance.startingHeroes = _availableHeroes;
            GameManager.Instance.addCombatRewards();
            CombatManager.Instance.ChangeCombatState(CombatState.CombatEnd);
            MenuManager.Instance.openVictoryScreen();
        }
        else if (CombatManager.Instance._turnQueue.ToList().FindAll(x => x.Faction == Faction.Hero).Count == 0)
        {
            SceneManager.LoadScene("GameOver");
        }
    }
    
    
}
