using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Rage", menuName = "Ability/Rage")]

public class Rage : Ability
{
    public int attackdamageIncrease = 1;

    public override void Activate(BaseUnit self)
    {
        Debug.Log("You now deal " + attackdamageIncrease + " more damage for the rest of the fight");
        self.changeDamage(attackdamageIncrease);
    }
}
