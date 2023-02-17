using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rouge : BaseHero
{
  // Unit Stats are copied from scriptable Unit
    private void Start() {
      standAction = true;
      StandActionName = "Hide";
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
