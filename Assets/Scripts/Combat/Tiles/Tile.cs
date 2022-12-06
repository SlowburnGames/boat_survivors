using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class Tile : MonoBehaviour
{
    [SerializeField] private bool isWalkable;
    [SerializeField] private Material[] normal = new Material[2];
    [SerializeField] private Material[] highlight = new Material[2];

    public string tileName;
    public BaseUnit tileUnit;
    public Vector2Int position;

    public List<GameObject> tilesToWalk = new List<GameObject>();

    public bool walkable => isWalkable && tileUnit == null;

    public void OnMouseEnter()
    {
        if (highlight.Length > 1)
            gameObject.GetComponent<MeshRenderer>().materials = highlight;
        else
            gameObject.GetComponent<MeshRenderer>().material = highlight[0];

        if(UnitManager.Instance.selectedHero != null)
            showPath(UnitManager.Instance.selectedHero.occupiedTile.GetComponent<Tile>());
        
        MenuManager.Instance.ShowTileInfo(this);
    }
    
    public void OnMouseExit()
    {
        if (normal.Length > 1)
            gameObject.GetComponent<MeshRenderer>().materials = normal;
        else
            gameObject.GetComponent<MeshRenderer>().material = normal[0];

        hidePath();

        MenuManager.Instance.ShowTileInfo(null);
    }

    private void OnMouseDown()
    {
        if (CombatManager.Instance.combatState == CombatState.HeroesTurn)
        {
            if (tileUnit != null)
            {
                // A hero is on the tile
                if (tileUnit.Faction == Faction.Hero)
                {
                    UnitManager.Instance.SetSelectedHero((BaseHero) tileUnit);
                    Debug.Log("Selected Hero: " + ((BaseHero) tileUnit).name);
                }
                else
                {
                    // When we next click on an enemy -> Attack it
                    if (UnitManager.Instance.selectedHero != null)
                    {
                        var enemy = (BaseEnemy) tileUnit;
                        // TODO Damage enemy in any form
                        Destroy(enemy.gameObject);
                        UnitManager.Instance.SetSelectedHero(null);
                        
                        Debug.Log("Damaged Enemy: " + enemy.name);
                    }
                }
            }
            else
            {
                // When we next click on an empty tile -> Move Hero to this tile
                if (UnitManager.Instance.selectedHero != null && isWalkable)
                {
                    if(calculatedDistance(UnitManager.Instance.selectedHero.occupiedTile.gameObject) <= UnitManager.Instance.selectedHero.MovementRange)
                    {
                        SetUnit(UnitManager.Instance.selectedHero);
                        UnitManager.Instance.SetSelectedHero(null);
                    }
                    else
                    {
                        Debug.LogError("cant move this far (distance: " + calculatedDistance(UnitManager.Instance.selectedHero.occupiedTile.gameObject) + ")");
                    }
                }
                else
                {
                    Debug.LogError("cant move there");
                }
            }
        }
    }

    public int calculatedDistance(GameObject tile)
    {
        
        int distanceX = (Mathf.Abs(tile.GetComponent<Tile>().position.x - this.GetComponent<Tile>().position.x));
        int distanceY = (Mathf.Abs(tile.GetComponent<Tile>().position.y - this.GetComponent<Tile>().position.y));

        int distance = Mathf.Abs(distanceY + distanceX);

        return distance;
    }

    public void showPath(Tile end)
    {
        Tile start = this.GetComponent<Tile>();
        

        if(start.position.x <= end.position.x)
        {
            for (int i = start.position.x; i <= end.position.x; i++)
            {
                tilesToWalk.Add(WFCGenerator.Instance._tiles[i][start.position.y].gameObject);
            }
        }
        else
        {
            for (int i = end.position.x; i <= start.position.x; i++)
            {
                tilesToWalk.Add(WFCGenerator.Instance._tiles[i][start.position.y].gameObject);
            }
        }
        if (start.position.y <= end.position.y)
        {
            for (int i = start.position.y; i <= end.position.y; i++)
            {
                tilesToWalk.Add(WFCGenerator.Instance._tiles[end.position.x][i].gameObject);
            }
        }
        else
        {
            for (int i = end.position.y; i <= start.position.y; i++)
            {
                tilesToWalk.Add(WFCGenerator.Instance._tiles[end.position.x][i].gameObject);
            }
        }

        Color c = Color.green;

        if(calculatedDistance(UnitManager.Instance.selectedHero.occupiedTile.gameObject) <= UnitManager.Instance.selectedHero.MovementRange)
        {
            c = Color.green;
        }
        else
        {
            c = Color.red;
        }

        foreach (var t in tilesToWalk)
        {
            Debug.Log("Tile: " + t.name);
            t.transform.GetChild(0).GetComponent<SpriteRenderer>().color = c;
            t.transform.GetChild(0).gameObject.gameObject.SetActive(true);
            t.transform.GetChild(0).localPosition = new Vector3(0, 0.6f, 0);
        }

    }

    public void hidePath()
    {
        foreach (var t in tilesToWalk)
        {
            t.transform.GetChild(0).gameObject.SetActive(false);
        }
        tilesToWalk.Clear();
    }

    public void SetUnit(BaseUnit unit)
    {
        if (unit.occupiedTile != null)
            unit.occupiedTile.tileUnit = null;
        unit.transform.position = transform.position + Vector3.up;
        unit.transform.LookAt(FindObjectOfType<Camera>().transform.position, Vector3.up);
        tileUnit = unit;
        unit.occupiedTile = this;
    }
    
}
