using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Stealth", menuName = "Ability/Stealth")]

public class Stealth : Ability
{
    public int duration = 1;
    public override void Activate(BaseUnit unit)
    {
        Debug.Log("You are now hidden for " + duration);
        unit.invisible = true;
        unit.SetInvisibilityEffect(true);
    }
}
