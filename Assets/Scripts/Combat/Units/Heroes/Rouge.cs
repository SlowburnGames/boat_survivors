using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rouge : BaseHero
{
    public void Reset()
    {
        Faction = Faction.Hero;
        UnitName = "Rouge";
        _maxHealth = 2;
        _moveDistance = 5;
    }

  public override void AttackTarget(BaseUnit target)
  {
    if(invisible)
    {
        target.Attack(AttackDamage * 2);
        invisible = false;
        return;
    }
    target.Attack(AttackDamage);

  }

  public override void SpecialMove(Tile target)
  {
    Debug.Log("Rogue Goes Invisible:");
    invisible = true;
    tilesWalkedThisTurn = 5;
  }
}
