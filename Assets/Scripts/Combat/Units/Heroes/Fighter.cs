using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fighter : BaseHero
{ 
    // Unit Stats are copied from scriptable Unit
    public override void AttackTarget(BaseUnit target)
    {
        AttacksMade--;
        MenuManager.Instance.updateAttacks(this);
        
        //move target 1 away of hero
        Vector2Int targetPos = target.OccupiedTile.position;
        Vector2Int attackDirection = WFCGenerator.getDirectionVector(OccupiedTile.position, targetPos);
        Vector2Int pushPos = targetPos += attackDirection;

        // Out of bounds check (kill pushed unit)
        if (pushPos.x < 0 || pushPos.x >= WFCGenerator.Instance._tiles[0].Length || pushPos.y < 0 ||
            pushPos.y >= WFCGenerator.Instance._tiles.Length)
        {
            
            target.TakeDamage(int.MaxValue);
        }
        // Nomal push
        else if (WFCGenerator.Instance._tiles[targetPos.x][targetPos.y].walkable)
        {
            target.TakeDamage(AttackDamage);
            UnitManager.Instance.SetUnit(target, WFCGenerator.Instance._tiles[targetPos.x][targetPos.y]);
        }
        // Water check (kill pushed unit)
        else if (WFCGenerator.Instance._tiles[targetPos.x][targetPos.y].tileName == "Water")
        {
            target.TakeDamage(int.MaxValue);
        }
        
        UnitManager.Instance.CheckAttackedUnit(target);
        
    }
}
