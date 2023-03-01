using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class Tile : MonoBehaviour
{
    [SerializeField] public bool isWalkable;
    [SerializeField] private Material[] normal = new Material[2];
    [SerializeField] private Material[] highlight = new Material[2];
    [SerializeField] private Color color = Color.white;
    private List<Material> materials;

    public string tileName;
    public BaseUnit tileUnit;
    private BaseHero _tileHeroPreview;

    public Vector2 tilePosition;
    public Vector2Int position;

    public List<Tile> tilesToWalk = new List<Tile>();

    public bool walkable {
        get { return isWalkable && tileUnit == null;}
        set { walkable = value; }
    }

    //Pathfinding Variables
    public int gCost;
    public int hCost;
    public int fCost {
        get {
            return gCost + hCost;
        }
    }

    public Tile parent;

    private void Awake()
    {
        materials = new List<Material>();
        materials.AddRange(new List<Material>(this.GetComponent<MeshRenderer>().materials));
    }
    
    public void OnMouseEnter()
    {
        foreach (var material in materials)
        {
            material.EnableKeyword("_EMISSION");
            material.SetColor("_EmissionColor", color);
        }
        // if (highlight.Length > 1)
        //     gameObject.GetComponent<MeshRenderer>().materials = highlight;
        // else
        //     gameObject.GetComponent<MeshRenderer>().material = highlight[0];
        
        if (CombatManager.Instance.combatState == CombatState.SpawnHeroes &&isWalkable && tileUnit == null)
            SpawnHeroPreview();

        if(UnitManager.Instance.selectedHero != null)
            showPath(UnitManager.Instance.selectedHero.OccupiedTile.GetComponent<Tile>());
        
        
        MenuManager.Instance.ShowTileInfo(this);
    }
    
    
    public void OnMouseExit()
    {
        foreach (var material in materials)
        {
            material.DisableKeyword("_EMISSION");
        }
        // if (normal.Length > 1)
        //     gameObject.GetComponent<MeshRenderer>().materials = normal;
        // else
        //     gameObject.GetComponent<MeshRenderer>().material = normal[0];
        
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
        if (GameManager.Instance.startingHeroes.Count == 0)
            Debug.LogError("No Heroes alive! (or set in the game manager)");

        var heroPreviewPrefab = UnitManager.Instance.getNextHero();
        
        _tileHeroPreview = Instantiate(heroPreviewPrefab);
        _tileHeroPreview.GetComponentInChildren<Canvas>().gameObject.SetActive(false);
        
        UnitManager.Instance.SetUnitPositionRotation(_tileHeroPreview, this, true);
        // var unitTransform = _tileHeroPreview.transform;
        // Vector3 unitYOffset = new Vector3(0, unitTransform.position.y, 0);
        // unitTransform.position = transform.position + Vector3.up/2;  // Vector.up/2 for the tile block
        // unitTransform.rotation = Quaternion.LookRotation(unitTransform.position + Vector3.up - cameraPos);
    }


    public int calculatedDistance(GameObject tile)
    {
        
        int distanceX = (Mathf.Abs(tile.GetComponent<Tile>().position.x - this.GetComponent<Tile>().position.x));
        int distanceY = (Mathf.Abs(tile.GetComponent<Tile>().position.y - this.GetComponent<Tile>().position.y));

        int distance = Mathf.Abs(distanceY + distanceX);

        return distance;
    }

    public void showPath(Tile start)
    {
        Tile end = this.GetComponent<Tile>();

        tilesToWalk = Pathfinding.Instance.FindPath(start, end);

        Color c = Color.green;

        if(tilesToWalk.Count <= UnitManager.Instance.selectedHero.MoveDistance - UnitManager.Instance.selectedHero.tilesWalked)
        {
            c = Color.green;
        }
        else
        {
            c = Color.red;
        }

        int rotation;
        Tile lastTile = start;
        
        //Debug.Log("Last TILE: " + tilesToWalk.Last().position);
        for (int i=0; i<tilesToWalk.Count; i++)
        {
            if (i != 0)
                lastTile = tilesToWalk[i - 1];
            
            Tile t = tilesToWalk[i];
            
            var walkingDir = findWalkingDirection(lastTile, t); // TODO check no next tile there (size)

            rotation = getRotation(walkingDir);
            
            var arrow = t.transform.GetChild(0);
            
            arrow.GetComponent<SpriteRenderer>().color = c;
            arrow.gameObject.gameObject.SetActive(true);
            arrow.localPosition = new Vector3(0, 0.6f, 0);
            arrow.rotation = Quaternion.Euler(new Vector3(-90, 0, rotation));
        }

    }

    private Vector2 findWalkingDirection(Tile start, Tile end)
    {
        return end.position - start.position;
    }

    private int getRotation(Vector2 walkingDir)
    {
        if (walkingDir.x < 0 && walkingDir.y < 0)
            return 135;//
        if (walkingDir.x < 0 && walkingDir.y > 0)
            return -135;
        if (walkingDir.x > 0 && walkingDir.y < 0)
            return 45;
        if (walkingDir.x > 0 && walkingDir.y > 0)
            return -45;//
        if (walkingDir.x < 0)
            return 180;
        if (walkingDir.y < 0)
            return 90;
        if (walkingDir.y > 0)
            return -90;

        return 0;   // default rotation (or walkingDir.x > 0)
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
