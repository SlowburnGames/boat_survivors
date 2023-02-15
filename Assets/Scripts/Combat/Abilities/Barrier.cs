using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Barrier", menuName = "Ability/Barrier")]

public class Barrier : Ability
{
    public int damageBlocked = 2;
    private int duration = 2;
    public override void Activate(BaseUnit self)
    {
        Debug.Log("Next time you take " + damageBlocked + " less damage");
        self.changeBlock(damageBlocked);
        self.effects.Add(new EffectList { id = 0, name = EffectNames.Barrier, turnsLeft = duration });
    }
}
