using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class GridManager : MonoBehaviour
{
    public static GridManager Instance;
    [SerializeField] private int gridWidth;
    [SerializeField] private int gridHeight;
    [SerializeField] private Tile tilePrefab;

    private Dictionary<Vector2, Tile> _tiles;

    private void Awake()
    {
        Instance = this;
    }

    public void GenerateGrid()
    {
        _tiles = new Dictionary<Vector2, Tile>();
        for (int x = 0; x < gridWidth; x++)
            for (int z = 0; z < gridHeight; z++)
            {
                var spawnedTile = Instantiate(tilePrefab, new Vector3(x, 0, z), quaternion.identity);
                spawnedTile.name = $"Tile {x} {z}";

                spawnedTile.Init((x % 2 == 0 && z % 2 != 0) || (x % 2 != 0 && z % 2 == 0));
                _tiles[new Vector2(x, z)] = spawnedTile;
            }
        
        CombatManager.Instance.ChangeCombatState(CombatState.SpawnHeroes);
    }

    public Tile GetHeroSpawnTile()
    {
        return _tiles.Where(t => t.Key.x < gridWidth/2 && t.Value.walkable).OrderBy(t => Random.value).First().Value;
    }
    
    public Tile GetEnemySpawnTile()
    {
        return _tiles.Where(t => t.Key.x > gridWidth/2 && t.Value.walkable).OrderBy(t => Random.value).First().Value;
    }

    public Tile GetTileAtPos(Vector2 pos)
    {
        if (_tiles.TryGetValue(pos, out var tile)) return tile;
        return null;
    }
}
