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

    public Vector2 tilePosition;
    public bool walkable => isWalkable && tileUnit == null;

    public void OnMouseEnter()
    {
        if (highlight.Length > 1)
            gameObject.GetComponent<MeshRenderer>().materials = highlight;
        else
            gameObject.GetComponent<MeshRenderer>().material = highlight[0];
        
        MenuManager.Instance.ShowTileInfo(this);
    }
    
    public void OnMouseExit()
    {
        if (normal.Length > 1)
            gameObject.GetComponent<MeshRenderer>().materials = normal;
        else
            gameObject.GetComponent<MeshRenderer>().material = normal[0];
        
        MenuManager.Instance.ShowTileInfo(null);
    }

    private void OnMouseDown()
    {
        if (CombatManager.Instance.combatState == CombatState.HeroesTurn)
        {
            UnitManager.Instance.HeroesTurn(this, tileUnit, isWalkable);
        }
    }

}
