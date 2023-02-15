using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fighter : BaseHero
{
    public void Reset()
    {
        faction = Faction.Hero;
        UnitName = "Fighter";
        _maxHealth = 3;
    }

  public override void AttackTarget(BaseUnit target)
  {
    base.AttackTarget(target);

    //move target 1 away of hero
    Vector2Int target_pos = target.OccupiedTile.position;

    Vector2Int attackDirection = WFCGenerator.getDirectionVector(OccupiedTile.position, target_pos);

    target_pos = target_pos + attackDirection;

    if(WFCGenerator.Instance._tiles[target_pos.x][target_pos.y].walkable)
    {
        UnitManager.Instance.SetUnit(target, WFCGenerator.Instance._tiles[target_pos.x][target_pos.y]);
    }
    
    if(WFCGenerator.Instance._tiles[target_pos.x][target_pos.y].tileName == "Water")
    {
        target.TakeDamage(int.MaxValue);
    }
  }
}
