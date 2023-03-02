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
            SetUnit(spawnedHero, spawnTile, true);
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
        
            SetUnit(spawnedEnemy, randomSpawnTile, true);
            CombatManager.Instance._spawnedUnitList.Add(spawnedEnemy);
        }
        CombatManager.Instance.ChangeCombatState(CombatState.SpawnHeroes);
    }

    public void HeroesTurn(Tile tile, BaseUnit tileUnit, bool isWalkable)
    {
        Debug.Log("HEROES TURN");
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
                            // Debug.Log("Damaged " + enemy.name + " by " + selectedHero.AttackDamage);

                            //selectedHero.usedAction = true;

                            // End turn
                            // ToggleAttackRangeIndicator(selectedHero, false);
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
                if (path.Count <= selectedHero.MoveDistance - selectedHero.tilesWalked)
                {
                    ToggleAttackRangeIndicator(selectedHero, false);
                    selectedHero.OccupiedTile.isWalkable = true;
                    StartCoroutine(MoveUnit(selectedHero, path, tile));
                    selectedHero.tilesWalked += path.Count;
                    
                    MenuManager.Instance.ShowSelectedHero(selectedHero);
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

    private bool OnlyHiddenRogueLeft(List<BaseUnit> heroes)
    {
        if (heroes.Count == 1 && heroes[0].Invisible)
            return true;
        return false;
    }
    
    public IEnumerator EnemiesTurn(BaseEnemy enemy)
    {
        float time = 2f;

        Debug.Log("Enemy turn!");
        //MenuManager.Instance.StartCoroutine(MenuManager.Instance.displayNextUnit(time / 2));
        yield return new WaitForSeconds(time/2);
        var target = findNearestHero(enemy);

        BaseUnit targetedHero = target.Item1;
        int distance = target.Item2;
        List<Tile> path = target.Item3;
        List<BaseUnit> allHeroes = target.Item4;

        if (!OnlyHiddenRogueLeft(allHeroes))   // When only (hidden) rogue is left no targetHero is found
        {
            //if enemy is reachable with melee attack
            if(distance <= enemy.MoveDistance)
            {
                bool attackedThisTurn = false;
                if(distance > 1)
                {
                    var newTile = findTileToWalk(enemy, path.GetRange(0, path.Count-1));
                    if (newTile == null)
                    {
                        // Stand still
                        newTile = enemy.OccupiedTile;
                    }
                    
                    int maxCount = enemy.MoveDistance < path.Count ? enemy.MoveDistance : path.Count;
                    var walkingPath = path.GetRange(0, maxCount-1);
                    
                    // SetUnit(enemy, newTile, false);
                    StartCoroutine(MoveUnit(enemy, walkingPath, newTile, targetedHero));
                    attackedThisTurn = true;
                }
                yield return new WaitForSeconds(time/2);
                if(!attackedThisTurn)
                {
                    enemy.AttackTarget(targetedHero);
                }
            }
            else //hero is not reachable
            {
                Debug.Log("Hero not reachable");
                if (!heroesAlive()) // When all heroes are dead -> Game over
                {
                    yield return new WaitForSeconds(time);
                    yield break;
                }

                int maxCount = enemy.MoveDistance < path.Count ? enemy.MoveDistance : path.Count;
                var findingPath = path.GetRange(0, maxCount);
                
                var newTile = findTileToWalk(enemy, findingPath);
                if (newTile == null)
                {
                    // Stand still
                    newTile = enemy.OccupiedTile;
                }
                var walkingPath = path.GetRange(0, maxCount);
                // SetUnit(enemy, newTile, false);
                
                yield return new WaitForSeconds(time/2);
                StartCoroutine(MoveUnit(enemy, walkingPath, newTile, targetedHero));
            }
        }

        yield return new WaitForSeconds(time / 4);
        checkCombatOver();
        CombatManager.Instance.ChangeCombatState(CombatState.UnitTurn);
    }

    private Tile findTileToWalk(BaseEnemy enemy, List<Tile> path)
    {
        var pathCopy = new List<Tile>(path);
        pathCopy.Reverse();
        
        foreach (var tile in pathCopy)
            if (checkTile(enemy, tile))
                return tile;

        return null;
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

    Tuple<BaseUnit, int, List<Tile>, List<BaseUnit>> findNearestHero(BaseEnemy enemy)
    {
        List<BaseUnit> allHeroes = CombatManager.Instance._turnQueue.ToList().FindAll(unit => unit.Faction == Faction.Hero);

        if (allHeroes.Count == 1 && allHeroes.First().Invisible)    // only invisible rogue left PFUSCH
            return Tuple.Create(allHeroes.First(), 0, new List<Tile>(), allHeroes);
        
        BaseUnit nearestHero = null;
        int minDistance = Int32.MaxValue;
        List<Tile> best_path = new List<Tile>();

        foreach (var hero in allHeroes)
        {
            if(hero.invisible)
                continue;

            List<Tile> path = Pathfinding.Instance.FindPath(enemy.OccupiedTile, hero.OccupiedTile, true);
            if(minDistance > path.Count)
            {
                minDistance = path.Count;
                nearestHero = hero;
                best_path = path;
            }
        }

        return Tuple.Create(nearestHero, minDistance, best_path, allHeroes);

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
            Color col = new Color((float)0.157, (float)0.2431, (float)0.8314);
            tilePos.transform.GetChild(1).GetComponent<SpriteRenderer>().color = col;
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

    public bool checkTile(BaseUnit unit, Tile tile)
    {
        if (tile.tileUnit != null)
        {
            Debug.Log("Tile " + tile.position + " already occupied!");
            return false;
        }

        return true;
    }
    
    public void SetUnit(BaseUnit unit, Tile tile, bool isUnitSpawning)
    {
        if (unit.OccupiedTile != null)
            unit.OccupiedTile.tileUnit = null;
        
        SetUnitPositionRotation(unit, tile, isUnitSpawning);

        tile.tileUnit = unit;
        unit.OccupiedTile = tile;
        CombatManager.Instance.UpdateTilesWalkability();
        MenuManager.Instance.UpdateHealthBar(unit);
    }

    public void SetUnitPositionRotation(BaseUnit unit, Tile tile, bool isUnitSpawning)
    {
        var unitTransform = unit.transform;

        Vector3 spawnOffset = isUnitSpawning ? Vector3.up / 2 : Vector3.zero;
        
        // Get unit y position because not every drawn hero sprite had same y position, and I noticed it too late.
        // -> change the unit prefab y-position, in a way that it "stands" on the 0-y-coord like intended. (see sorceress, rogue, skeleton, ...)
        Vector3 unitYOffset = new Vector3(0, unitTransform.position.y, 0);
        unitTransform.position = tile.transform.position + unitYOffset + spawnOffset;  // Vector.up/2 for the tile block
        
        unitTransform.rotation = Quaternion.Euler(new Vector3(10, -45, 0));

    }
    
    public IEnumerator MoveUnit(BaseUnit unit, List<Tile> path, Tile tile, BaseUnit defender = null)
    {
        float time = 0.5f;
        Vector3[] waypoints = new Vector3[path.Count];
        Vector3 unitYOffset = new Vector3(0, unit.transform.position.y, 0);
        for (int i = 0; i < path.Count; i++)
        {
            //waypoints[i] = path[i].transform.position + Vector3.up;
            waypoints[i] = path[i].transform.position + unitYOffset + Vector3.zero;
        }
        unit.GetComponent<MovementManager>().moveUnit(waypoints, time);
        yield return new WaitForSeconds(time);
        SetUnit(unit, tile, false);

        if (unit.Faction == Faction.Hero)
            ToggleAttackRangeIndicator((BaseHero)unit, true);

        if(unit.Faction == Faction.Enemy && CheckAttackPossible(unit, defender))
            unit.AttackTarget(defender);
    }

    private bool CheckAttackPossible(BaseUnit attackUnit, BaseUnit defendUnit)
    {
        if(defendUnit == null)
        {
            return false;
        }

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
    
    public void CheckAttackedUnit(BaseUnit aUnit)
    {
        if (aUnit.Health <= 0)
        {
            CombatManager.Instance._turnQueue =
                new Queue<BaseUnit>(CombatManager.Instance._turnQueue.Where(x => x != aUnit));
            if(aUnit.Faction == Faction.Hero)
            {
                int index = _availableHeroes.FindIndex(a => a.unitID == aUnit.unitID);
                _availableHeroes.RemoveAt(index);
                
                GameManager.Instance.combatMoraleReward -= 10;
                GameManager.Instance.combatResReward -= 10;
            }

            aUnit.OccupiedTile.isWalkable = true;
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
            // CombatManager.Instance.ChangeCombatState(CombatState.CombatEnd);
            SceneManager.LoadScene("GameOver");
        }
    }
    
    public void normalizeMoveArrowRatations()
    {
        // Debug only START
        // foreach (var tileRow in WFCGenerator.Instance._tiles)
        //     foreach (var tile in tileRow)
        //         tile.transform.GetChild(0).gameObject.SetActive(true);
        // Debug only END
        
        foreach (var tileRow in WFCGenerator.Instance._tiles)
            foreach (var tile in tileRow)
                tile.transform.GetChild(0).rotation = Quaternion.Euler(new Vector3(-90, 0, 0));
        
        // Needed because all tiles (and therefore also arrows) have random rotations at start
    }

    public void resetUnit()
    {
        selectedHero.tilesWalked = 0;
        selectedHero.AttacksMade = selectedHero.MaxAttacks;
        ToggleAttackRangeIndicator(selectedHero, false);
        SetSelectedHero(null);
    }
    
    
}
