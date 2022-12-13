using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using Random = UnityEngine.Random;

[System.Serializable]
public class Cell
{
    public bool isCollapsed = false;
    public List<int> options = new List<int>();
    public Vector3 pos;
    public TilePrefab prefab;
}

[System.Serializable]
public class TilePrefab
{
    public GameObject tile;
    public Quaternion rotation;
    public List<string> edges;

    public string name;
}

public class BactraceList
{
    public List<Cell> backtraceList = new List<Cell>();
    public int step;
    public int lastModified;
}

public class WFCGenerator : MonoBehaviour
{
    public static WFCGenerator Instance;

    public List<TilePrefab> baseTiles;
    private int startingTileCount = 0;
    public GameObject blank;
    //public List<TilePrefab> tiles;
    private float dim;

    private List<int> bactraceIndices = new List<int>();
    private List<BactraceList> bactraceList = new List<BactraceList>();

    private List<int> optionsCount = new List<int>();
    public List<int> optionsLeft = new List<int>(); 

    public float playbackSpeed = 0.005f;
    public bool animated = true;
    private bool gridReady = false;
    public bool needRotation = false;

    public Vector2 gridSize = new Vector2(5, 5);

    public List<Vector2> gridPos = new List<Vector2>();
    public List<GameObject> gridTiles = new List<GameObject>();
    public List<Cell> tileObjects = new List<Cell>();

    public List<List<int>> rotatedTiles = new List<List<int>>(); 

    public Tile[][] _tiles;

    [Header("MapGenSettings")]
    public bool noSmallLakes;
    public bool noWater;
    public bool noSand;
    public bool noGrass;

    [Header("MapGenPrefabs")]
    public bool beach;


    // Start is called before the first frame update

    void Start()
    {
        //generateTileSet();
    }

    private void Awake()
    {
        Instance = this;
    }

    public void generateTileSet()
    {
        startingTileCount = baseTiles.Count;
        if (needRotation)
        {
            int count = baseTiles.Count;
            for (int i = 0; i < count; i++)
            {
                rotatedTiles.Add(new List<int>());
                for (int j = 0; j < 4; j++)
                {
                    List<string> oldEdges = new List<string>(baseTiles[i].edges);
                    List<string> newEdges = new List<string>();
                    Quaternion rotation = new Quaternion();
                    rotation = Quaternion.Euler(0, -90f * (j + 1), 0);
                    for (int k = 0; k < 4; k++)
                    {
                        //newTile.edges[k] = (oldEdges[(k + 1*(j)) % 4]);
                        newEdges.Add((oldEdges[(k + 1 * (j + 1)) % 4]));
                    }
                    for (int k = 0; k < 4; k++)
                    {
                        if (oldEdges[k] != newEdges[k])
                        {
                            baseTiles.Add(new TilePrefab { tile = baseTiles[i].tile, edges = newEdges, rotation = rotation, name = baseTiles[i].name });
                            rotatedTiles[i].Add(baseTiles.Count - 1);
                            break;
                        }
                    }
                }
            }
        }
        for (int i = 0; i < baseTiles.Count; i++)
        {
            optionsCount.Add(i);
            optionsLeft.Add(99999);
        }
        //Debug.Log("Options count: " + optionsCount.Count);
    }

    public void createGrid()
    {
        generateTileSet();
        
        // init sorted tiles array
        _tiles = new Tile[(int)gridSize.x][];
        for (int i = 0; i < (int)gridSize.x; i++)
            _tiles[i] = new Tile[(int)gridSize.y];
        
        clearGrid();

        for (int i = 0; i < gridSize.x; i++)
        {
            for (int j = 0; j < gridSize.y; j++)
            {
                int tileIndex = baseTiles.Count - 1;
                //dim = baseTiles[tileIndex].tile.gameObject.GetComponent<Renderer>().bounds.size.x;
                dim = 1;
                //Vector3 newPos = new Vector3(dim * i - (dim * gridSize.x) / 2, 0,  dim * j - (dim * gridSize.y) / 2);
                Vector3 newPos = new Vector3(i, 0, j);
                GameObject tile = Instantiate(blank, new Vector3(newPos.x, 0, newPos.z), Quaternion.identity, this.transform);
                //tile.GetComponent<Tile>().index = gridTiles.Count;
                gridTiles.Add(tile);
                tileObjects.Add(new Cell { isCollapsed = false, options = new List<int>(optionsCount), pos = newPos, prefab = new TilePrefab { tile = tile, edges = baseTiles[tileIndex].edges, rotation = baseTiles[tileIndex].rotation, name = baseTiles[tileIndex].name } });
            }
        }
        gridReady = true;
        applyRulsets();
    }

    public void applyRulsets()
    {
        if (noWater)
        {
            for (int j = 0; j < startingTileCount; j++)
            {
                for (int i = 0; i < baseTiles[j].edges.Count; i++)
                {
                    if (baseTiles[j].edges[i].Contains("W"))
                    {
                        restrictOptions(j, 0);
                    }
                }
            }
        }
        if (noSand)
        {
            for (int j = 0; j < startingTileCount; j++)
            {
                for (int i = 0; i < baseTiles[j].edges.Count; i++)
                {
                    if (baseTiles[j].edges[i].Contains("S"))
                    {
                        restrictOptions(j, 0);
                    }
                }
            }
        }
        if (noGrass)
        {
           // restrictOptions(0, -1, "half");
            for (int j = 0; j < startingTileCount; j++)
            {
                for (int i = 0; i < baseTiles[j].edges.Count; i++)
                {
                    if (baseTiles[j].edges[i].Contains("G"))
                    {
                        restrictOptions(j, 0);
                        //restrictOptions(j, -1, "half");
                    }
                }
            }
        }

        if (beach)
        {
            for (int i = 0; i < gridSize.x; i++)
            {
                if(i%2 ==0)
                {
                    int min = (int)gridSize.x, max = (int)(gridSize.x * 2);
                    collapsCell(Random.Range(min, max), 4);
                }
                collapsCell(i, 4);
            }
            for (int i = (int)gridSize.x; i < (int)gridSize.x * 2; i++)
            {
                //removeOption(i, 4);
            }
            //restrictOptions(4, 0);
        }

    }

    public void runWFC()
    {
        if (!gridReady)
        {
            createGrid();
        }

        if (animated)
        {
            //StartCoroutine(collapseGridAnimation());
        }
        else
        {
           collapseGrid();
        }
        gridReady = false;

        CombatManager.Instance.ChangeCombatState(CombatState.SpawnEnemies);
    }

    public void collapseGrid()
    {
        //bool failed = false;
        int backTraces = 5;
        int lastIndex = 0;
        for (int i = 0; i < gridSize.x * gridSize.y; i++)
        {
            //check options
            for (int j = 0; j < optionsLeft.Count; j++)
            {
                if (optionsLeft[j] == 0)
                {
                    clearOptionsOfTyp(j);
                }
            }

            Cell newCell = selectTileWithLowestEntropy();
            if (newCell == null)
            {
                break;
            }
            int index = tileObjects.IndexOf(newCell);
            List<int> options = tileObjects[index].options;
            if (options.Count == 0)
            {
                tileObjects = bactraceList[^backTraces].backtraceList;
                i = bactraceList[^backTraces].step - 1;
                backTraces++;
                continue;
                //failed = true;
                //break;
            }
            backTraces = 5;
            int newOption = options[Random.Range(0, options.Count)];
            optionsLeft[newOption]--;
            tileObjects[index].options = new List<int> { newOption };
            tileObjects[index].isCollapsed = true;
            tileObjects[index].prefab.rotation = baseTiles[newOption].rotation;
            Vector3 pos = tileObjects[index].pos;
            GameObject tile = Instantiate(baseTiles[newOption].tile, new Vector3(pos.x, 0, pos.z), baseTiles[newOption].rotation, this.transform);

            tile.name = $"Tile {pos.x} {pos.z}";
            Tile tempTile = tile.GetComponent<Tile>();
            _tiles[(int)pos.x][(int)pos.z] = tempTile;
            tempTile.tilePosition = new Vector2(pos.x, pos.z);
            tile.GetComponent<Tile>().position = new Vector2Int((int)pos.x, (int)pos.z);
            _tiles[(int)pos.x][(int)pos.z] = tile.GetComponent<Tile>();

            tileObjects[index].prefab.tile = tile;
            tileObjects[index].prefab.name = baseTiles[newOption].name;
            tileObjects[index].prefab.edges = baseTiles[newOption].edges;
            Destroy(gridTiles[index].gameObject);
            gridTiles[index] = tile;
            bactraceList.Add(new BactraceList { backtraceList = deepCopyList(tileObjects), step = i, lastModified = lastIndex });
            getTilesToUpdate(index);
            lastIndex = index;
        }

        //if (failed)
        //{
        //    Debug.Log("Failed");
        //    createGrid();
        //    collapseGrid();
        //}
    }

    public void collapsCell(int index, int baseTileIndex)
    {
        if (!tileObjects[index].options.Contains(baseTileIndex))
        {
            Debug.LogError("Cant place tile there");
            return;
        }
        List<int> options = tileObjects[index].options;

        //check options
        for (int i = 0; i < optionsLeft.Count; i++)
        {
            if (optionsLeft[i] == 0)
            {
                clearOptionsOfTyp(i);
            }
        }

        if (options.Count == 0)
        {
            return;
        }
        int newOption = baseTileIndex;
        optionsLeft[newOption]--;
        tileObjects[index].options = new List<int> { newOption };
        tileObjects[index].prefab.rotation = baseTiles[newOption].rotation;
        tileObjects[index].isCollapsed = true;
        Vector3 pos = tileObjects[index].pos;
        GameObject tile = Instantiate(baseTiles[newOption].tile, new Vector3(pos.x, 0, pos.z), baseTiles[newOption].rotation, this.transform);

        tile.name = $"Tile {pos.x} {pos.z}";
        Tile tempTile = tile.GetComponent<Tile>();
        _tiles[(int)pos.x][(int)pos.z] = tempTile;
        tempTile.tilePosition = new Vector2(pos.x, pos.z);
        tile.GetComponent<Tile>().position = new Vector2Int((int)pos.x, (int)pos.z);
        _tiles[(int)pos.x][(int)pos.z] = tile.GetComponent<Tile>();

        tileObjects[index].prefab.tile = tile;
        tileObjects[index].prefab.name = baseTiles[newOption].name;
        tileObjects[index].prefab.edges = baseTiles[newOption].edges;
        Destroy(gridTiles[index].gameObject);
        gridTiles[index] = tile;
        getTilesToUpdate(index);
    }

    //public IEnumerator collapseGridAnimation()
    //{
    //    bool failed = false;
    //    int backTraces = 5;
    //    int lastIndex = 0;
    //    for (int i = 0; i < gridSize.x * gridSize.y; i++)
    //    {
    //        Cell newCell = selectTileWithLowestEntropy();
    //        if (newCell == null)
    //        {
    //            break;
    //        }
    //        int index = tileObjects.IndexOf(newCell);
    //        List<int> options = tileObjects[index].options;
    //        if (options.Count == 0)
    //        {
    //            //yield return new WaitForSeconds(5);
    //            //playbackSpeed = 5;
    //            //Debug.LogError("Needed to backtrace for tile " + index);
    //            tileObjects = bactraceList[^backTraces].backtraceList;
    //            i = bactraceList[^backTraces].step - 1;
    //            backTraces++;
    //            //Destroy(gridTiles[lastIndex].gameObject);
    //            continue;
    //            //failed = true;
    //            //break;
    //        }
    //        backTraces = 5;
    //        int newOption = options[UnityEngine.Random.Range(0, options.Count)];
    //        tileObjects[index].options = new List<int> { newOption };
    //        tileObjects[index].isCollapsed = true;
    //        tileObjects[index].prefab.rotation = baseTiles[newOption].rotation;
    //        Vector3 pos = tileObjects[index].pos;
    //        GameObject tile = Instantiate(baseTiles[newOption].tile, new Vector3(pos.x, 0, pos.z), baseTiles[newOption].rotation, this.transform);

    //        tile.name = $"Tile {pos.x} {pos.z}";
    //        _tiles[(int)pos.x][(int)pos.z] = tile.GetComponent<Tile>();

    //        tileObjects[index].prefab.tile = tile;
    //        tileObjects[index].prefab.edges = baseTiles[newOption].edges;
    //        Destroy(gridTiles[index].gameObject);
    //        gridTiles[index] = tile;
    //        bactraceList.Add(new BactraceList { backtraceList = deepCopyList(tileObjects), step = i, lastModified = lastIndex });
    //        getTilesToUpdate(index);
    //        lastIndex = index;
    //        yield return new WaitForSeconds(playbackSpeed);
    //    }
    //    //if (failed)
    //    //{

    //    //    //createGrid();
    //    //    //StartCoroutine(collapseGridAnimation());
    //    //}
    //}

    public void restrictOptions(int option, int count = -1, string area = "all")
    {
        if(count >= 0)
        {
            optionsLeft[option] = count;
            for (int i = 0; i < rotatedTiles[option].Count; i++)
            {
                Debug.Log("index: " + rotatedTiles[option][i]);
                optionsLeft[rotatedTiles[option][i]] = count;
            }
        }

        if(area == "half")
        {
            int tileCount = (int)(gridSize.x * gridSize.y);
            for (int i = Mathf.RoundToInt(tileCount/2); i < tileCount; i++)
            {
                Debug.Log("Removed option " + 0 + " from tile " + i);
                removeOption(i,option);
            }
        }
    }

    public void removeOption(int tileIndex, int option)
    {
        if (tileObjects[tileIndex].options.Exists(o => o == option))
        {
            //Debug.Log("Clearing option " + option);
            tileObjects[tileIndex].options.Remove(option);
        }
    }

    public void clearOptionsOfTyp(int option)
    {
        foreach (var t in tileObjects)
        {
            if(t.options.Exists(o => o == option))
            {
                //Debug.Log("Clearing option " + option);
                t.options.Remove(option);
            }
        }
    }

    public List<Cell> deepCopyList(List<Cell> list)
    {
        List<Cell> copyList = new List<Cell>();

        for (int i = 0; i < list.Count; i++)
        {
            Cell newCell = new Cell();
            newCell.isCollapsed = list[i].isCollapsed;
            List<int> newOptions = new List<int>();
            for (int j = 0; j < list[i].options.Count; j++)
            {
                newOptions.Add(list[i].options[j]);
            }
            newCell.options = newOptions;
            newCell.pos = list[i].pos;
            TilePrefab newTilePrefab = new TilePrefab();
            List<string> newEdges = new List<string>();
            for (int j = 0; j < list[i].prefab.edges.Count; j++)
            {
                newEdges.Add(list[i].prefab.edges[j]);
            }
            newTilePrefab.edges = newEdges;
            newTilePrefab.rotation = list[i].prefab.rotation;
            newTilePrefab.tile = list[i].prefab.tile;
            newTilePrefab.name = list[i].prefab.name;
            newCell.prefab = newTilePrefab;
            copyList.Add(newCell);
        }

        return copyList;
    }

    public void getTilesToUpdate(int index)
    {
        float arrayCount = gridSize.x * gridSize.y;
        if (!(index % gridSize.y == gridSize.y - 1))
        {
            updateOptions(index, index + 1, 0);
        }
        if (index < arrayCount - gridSize.y)
        {
            updateOptions(index, index + (int)gridSize.y, 1);
        }
        if (!(index % gridSize.y == 0))
        {
            updateOptions(index, index - 1, 2);
        }
        if (index >= gridSize.y)
        {
            updateOptions(index, index - (int)gridSize.y, 3);
        }
    }

    public void updateOptions(int oldIndex, int index, int dir)
    {
        if (tileObjects[index].isCollapsed)
        {
            return;
        }
        for (int i = 0; i < baseTiles.Count; i++)
        {
            string s1 = baseTiles[i].edges[(dir + 2 + 4) % 4];
            string s2 = reverseString(tileObjects[oldIndex].prefab.edges[dir]);

            for (int j = 0; j < s1.Length; j++)
            {
                if (s1[j] != s2[j])
                {
                    tileObjects[index].options.Remove(i);
                    break;
                }
            }

            if(noSmallLakes)
            {
                if(tileObjects[oldIndex].prefab.name == "CliffStraight" && baseTiles[i].name == "CliffStraight" && tileObjects[oldIndex].prefab.rotation.eulerAngles.y + baseTiles[i].rotation.eulerAngles.z == 180)
                {
                    //Debug.Log("Removed cliff straight");
                    tileObjects[index].options.Remove(i);
                }
                if (tileObjects[oldIndex].prefab.name == "CliffTurn" && baseTiles[i].name == "CliffTurn")
                {
                    tileObjects[index].options.Remove(i);
                }
                if (tileObjects[oldIndex].prefab.name == "CoastStraight" && baseTiles[i].name == "CoastStraight" && tileObjects[oldIndex].prefab.rotation.eulerAngles.y + baseTiles[i].rotation.eulerAngles.z == 180)
                {
                    //Debug.Log("Removed cliff straight");
                    tileObjects[index].options.Remove(i);
                }
                if (tileObjects[oldIndex].prefab.name == "CoastTurn" && baseTiles[i].name == "CoastTurn")
                {
                    tileObjects[index].options.Remove(i);
                }
                if (tileObjects[oldIndex].prefab.name == "TransitionStraight" && baseTiles[i].name == "TransitionStraight" && tileObjects[oldIndex].prefab.rotation.eulerAngles.y + baseTiles[i].rotation.eulerAngles.z == 180)
                {
                    //Debug.Log("Removed cliff straight");
                    tileObjects[index].options.Remove(i);
                }
                if (tileObjects[oldIndex].prefab.name == "TransitionTurn" && baseTiles[i].name == "TransitionTurn")
                {
                    tileObjects[index].options.Remove(i);
                }

            }

        }
    }

    public string reverseString(string s)
    {
        string returnString = "";
        for (int i = s.Length - 1; i >= 0; i--)
        {
            returnString += s[i];
        }
        return returnString;
    }

    public Cell selectTileWithLowestEntropy()
    {
        List<Cell> sortedTiles = tileObjects.OrderBy(t => t.options.Count).ToList();
        sortedTiles.RemoveAll(t => t.isCollapsed == true);
        if (sortedTiles.Count == 0)
        {
            return null;
        }

        for (int i = 0; i < sortedTiles.Count - 1; i++)
        {
            if (sortedTiles[i].options.Count < sortedTiles[i + 1].options.Count)
            {
                return sortedTiles[Random.Range(0, i)];
            }
        }
        return sortedTiles[Random.Range(0, sortedTiles.Count)];
    }

    public void clearGrid()
    {
        foreach (var tile in gridTiles)
        {
            Destroy(tile.gameObject);
        }
        gridTiles.Clear();
        tileObjects.Clear();
        bactraceList.Clear();
    }

    public void clearCell(int index)
    {
        Destroy(gridTiles[index].gameObject);
        gridTiles.RemoveAt(index);
        tileObjects.RemoveAt(index);

    }

    public void createBlankCell(Vector3 pos, int index)
    {
        int tileIndex = baseTiles.Count - 1;
        dim = baseTiles[tileIndex].tile.gameObject.GetComponent<Renderer>().bounds.size.x;
        GameObject tile = Instantiate(blank, new Vector3(pos.x, 0, pos.z), Quaternion.identity, this.transform);
        //tile.GetComponent<Tile>().index = index;
        gridTiles.Insert(index, tile);
        tileObjects.Insert(index, new Cell { isCollapsed = false, options = new List<int>(optionsCount), pos = pos, prefab = new TilePrefab { tile = tile, edges = baseTiles[tileIndex].edges, rotation = baseTiles[tileIndex].rotation, name = baseTiles[tileIndex].name } });
    }

    public Tile GetHeroSpawnTile()
    {
        var walkableTiles = GetAllWalkableTiles();
        return walkableTiles[Random.Range(0, walkableTiles.Count)];
    }

    public Tile GetEnemySpawnTile()
    {
        var walkableTiles = GetAllWalkableTiles();
        return walkableTiles[Random.Range(0, walkableTiles.Count)];
    }

    public List<Tile> GetAllWalkableTiles()
    {
        List<Tile> walkableTiles = new List<Tile>();
        for (int x = 0; x < gridSize.x; x++)
            for (int y = 0; y < gridSize.y; y++)
            {
                if (_tiles[x][y] != null && _tiles[x][y].walkable) 
                    walkableTiles.Add(_tiles[x][y]);
            }
        return walkableTiles;
    }
    public List<Tile> getNeighbours(Tile tile)
    {
        List<Tile> neighbours = new List<Tile>();

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if(x == 0 && y == 0)
                    continue;
                
                int checkX = tile.position.x + x;
                int checkY = tile.position.y + y;

                if(checkX >= 0 && checkX < gridSize.x
                    && checkY >= 0 && checkY < gridSize.y)
                {
                    neighbours.Add(_tiles[checkX][checkY]);
                }
            }
        }

        return neighbours;
    }


}
