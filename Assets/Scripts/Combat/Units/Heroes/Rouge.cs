using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rouge : BaseHero
{
    private void Start() {
      standAction = true;
      standActionName = "Hide";
    }
    public void Reset()
    {
        faction = Faction.Hero;
        UnitName = "Rouge";
        _maxHealth = 2;
        _moveDistance = 5;
    }

  public override void AttackTarget(BaseUnit target)
  {
    if(invisible)
    {
        target.TakeDamage(AttackDamage * 3);
        invisible = false;
        return;
    }
    target.TakeDamage(AttackDamage);

  }
}
