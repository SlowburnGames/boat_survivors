using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rogue : BaseHero
{
  // Unit Stats are copied from scriptable Unit
    private void Start() {
      standAction = true;
      StandActionName = "Stealth";
    }

  public override void AttackTarget(BaseUnit target)
  {
    AttacksMade--;
    int attackDmg = AttackDamage;
    if(invisible)
    {
        attackDmg = AttackDamage * 3;
        invisible = false;
        SetInvisibilityEffect(false);
    }
    target.TakeDamage(attackDmg);
    MenuManager.Instance.updateAttacks(this);
    UnitManager.Instance.CheckAttackedUnit(target);
  }
}
