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
    private BaseHero _tileHeroPreview;

    public Vector2 tilePosition;
    public bool walkable => isWalkable && tileUnit == null;

    public void OnMouseEnter()
    {
        if (highlight.Length > 1)
            gameObject.GetComponent<MeshRenderer>().materials = highlight;
        else
            gameObject.GetComponent<MeshRenderer>().material = highlight[0];
        
        if (CombatManager.Instance.combatState == CombatState.SpawnHeroes &&isWalkable && tileUnit == null)
            SpawnHeroPreview();
        
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

    private void DestroyHeroPreview()
    {
        Destroy(_tileHeroPreview.gameObject);
    }

}
