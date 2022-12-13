using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class Tile : MonoBehaviour
{
    [SerializeField] public bool isWalkable;
    [SerializeField] private Material[] normal = new Material[2];
    [SerializeField] private Material[] highlight = new Material[2];

    public string tileName;
    public BaseUnit tileUnit;
    private BaseHero _tileHeroPreview;

    public Vector2 tilePosition;
    public Vector2Int position;

    public List<GameObject> tilesToWalk = new List<GameObject>();

    public bool walkable => isWalkable && tileUnit == null;

    //Pathfinding Variables
    public int gCost;
    public int hCost;
    public int fCost {
        get {
            return gCost + hCost;
        }
    }

    public Tile parent;

    public void OnMouseEnter()
    {
        if (highlight.Length > 1)
            gameObject.GetComponent<MeshRenderer>().materials = highlight;
        else
            gameObject.GetComponent<MeshRenderer>().material = highlight[0];
        
        if (CombatManager.Instance.combatState == CombatState.SpawnHeroes &&isWalkable && tileUnit == null)
            SpawnHeroPreview();

        if(UnitManager.Instance.selectedHero != null)
            showPath(UnitManager.Instance.selectedHero.OccupiedTile.GetComponent<Tile>());
        
        
        MenuManager.Instance.ShowTileInfo(this);
    }
    
    public void OnMouseExit()
    {
        if (normal.Length > 1)
            gameObject.GetComponent<MeshRenderer>().materials = normal;
        else
            gameObject.GetComponent<MeshRenderer>().material = normal[0];
        
        if (CombatManager.Instance.combatState == CombatState.SpawnHeroes &&isWalkable && tileUnit == null)
            DestroyHeroPreview();
        

        hidePath();

        MenuManager.Instance.ShowTileInfo(null);
    }

    private void OnMouseDown()
    {
        if (CombatManager.Instance.combatState == CombatState.HeroTurn)
        {
            UnitManager.Instance.HeroesTurn(this, tileUnit, isWalkable);
        }
        if (tileUnit == null && CombatManager.Instance.combatState == CombatState.SpawnHeroes && isWalkable)
        {
            DestroyHeroPreview();
            UnitManager.Instance.SpawnSelectedHero(this);
        }
    }
    
    private void SpawnHeroPreview()
    {
        var heroPreviewPrefab = UnitManager.Instance.GetNextHero();
        _tileHeroPreview = Instantiate(heroPreviewPrefab);
        _tileHeroPreview.GetComponent<SpriteRenderer>().color -= new Color (0, 0, 0, 0.6f);
        _tileHeroPreview.GetComponentInChildren<Canvas>().gameObject.SetActive(false);
        _tileHeroPreview.transform.position = transform.position + Vector3.up;
        _tileHeroPreview.transform.LookAt(FindObjectOfType<Camera>().transform.position, Vector3.up);
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

        if(calculatedDistance(UnitManager.Instance.selectedHero.OccupiedTile.gameObject) <= UnitManager.Instance.selectedHero.MoveDistance)
        {
            c = Color.green;
        }
        else
        {
            c = Color.red;
        }

        foreach (var t in tilesToWalk)
        {
            //Debug.Log("Tile: " + t.name);
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

    private void DestroyHeroPreview()
    {
        Destroy(_tileHeroPreview.gameObject);
    }

}
