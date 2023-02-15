using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Rage", menuName = "Ability/Rage")]

public class Wisdom : Ability
{
    public int attackdamageIncrease = 1;
    public int duration;
    public void WiseUp()
    {
        Debug.Log("You now deal " + attackdamageIncrease + " more damage for " + duration);
    }
}
