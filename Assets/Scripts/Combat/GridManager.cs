using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;

public class GridManager : MonoBehaviour
{
    [SerializeField] private int gridWidth;
    [SerializeField] private int gridHeight;
    [SerializeField] private Tile tilePrefab;

    private void Start()
    {
        GenerateGrid();
    }

    void GenerateGrid()
    {
        for (int x = 0; x < gridWidth; x++)
            for (int z = 0; z < gridHeight; z++)
            {
                var spawnedTile = Instantiate(tilePrefab, new Vector3(x, 0, z), quaternion.identity);
                spawnedTile.name = $"Tile {x} {z}";

                spawnedTile.Init((x % 2 == 0 && z % 2 != 0) || (x % 2 != 0 && z % 2 == 0));
            }
    }
}
