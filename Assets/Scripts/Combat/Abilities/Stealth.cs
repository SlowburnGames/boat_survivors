using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Stealth", menuName = "Ability/Stealth")]

public class Stealth : Ability
{
    public int duration = 1;
    public override void Activate(BaseUnit self)
    {
        Debug.Log("You are now stealthed for " + duration);
        self.invisible = true;
    }
}
