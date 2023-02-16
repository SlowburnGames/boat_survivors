using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinding : MonoBehaviour
{
    public static Pathfinding Instance { get; private set; }

    public List<Tile> found_path;

    private void Awake()
    {
        Instance = this;
    }
    public List<Tile> FindPath(Tile start, Tile target)
    {   
        //Debug.Log("Pathfinding");
        Tile[][] grid = WFCGenerator.Instance._tiles;

        List<Tile> openSet = new List<Tile>();
        HashSet<Tile> closedSet = new HashSet<Tile>();

        openSet.Add(start);

        while(openSet.Count > 0)
        {
            Tile currentTile = openSet[0];
            for(int i = 1; i < openSet.Count; i++)
            {
                if(openSet[i].fCost < currentTile.fCost || (openSet[i].fCost == currentTile.fCost && openSet[i].hCost < currentTile.hCost))
                {
                    currentTile = openSet[i];
                }
            }

            openSet.Remove(currentTile);
            closedSet.Add(currentTile);

            if(currentTile == target)
            {
                return retracePath(start, target);
            }

            var neighbours = WFCGenerator.Instance.getNeighbours(currentTile);

            foreach (var neighbour in neighbours)
            {
                if(!neighbour.isWalkable || closedSet.Contains(neighbour))
                    continue;
                
                int newMovementCostToNeighbour = currentTile.gCost + getDistance(currentTile, neighbour);
                if(newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                {
                    neighbour.gCost = newMovementCostToNeighbour;
                    neighbour.hCost = getDistance(neighbour, target);
                    neighbour.parent = currentTile;

                    if(!openSet.Contains(neighbour))
                    {
                        openSet.Add(neighbour);
                    }
                }
            }
        }

        return new List<Tile>();
    }

    List<Tile> retracePath(Tile startTile, Tile endTile)
    {
        List<Tile> path = new List<Tile>();

        Tile currentTile = endTile;

        while(currentTile != startTile)
        {
            path.Add(currentTile);
            currentTile = currentTile.parent;
        }
        path.Add(currentTile);

        path.Reverse();
        path.RemoveAt(0);

        return path;
    }

    int getDistance(Tile a, Tile b)
    {
        int distanceX = Mathf.Abs(a.position.x - b.position.x);
        int distanceY = Mathf.Abs(a.position.y - b.position.y);

        if(distanceX > distanceY)
        {
            return 14 * distanceY + 10 * (distanceX - distanceY);
        }
        return 14 * distanceX + 10 * (distanceY - distanceX);
    }
}
