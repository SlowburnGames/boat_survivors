using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class Tile : MonoBehaviour
{
    [SerializeField] private bool isWalkable;

    public string tileName;
    public BaseUnit tileUnit;
    public bool walkable => isWalkable && tileUnit == null;

    public void OnMouseEnter()
    {
        transform.GetChild(0).gameObject.SetActive(true);
        MenuManager.Instance.ShowTileInfo(this);
    }
    
    public void OnMouseExit()
    {
        transform.GetChild(0).gameObject.SetActive(false);
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
                    SetUnit(UnitManager.Instance.selectedHero);
                    UnitManager.Instance.SetSelectedHero(null);
                    
                }
            }
        }
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
