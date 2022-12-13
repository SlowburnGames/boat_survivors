using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wizard : BaseHero
{
    public void Reset()
    {
        Faction = Faction.Hero;
        UnitName = "Wizard";
        _maxHealth = 90;
        _attackRange = 4;
        _isRanged = true;
    }
}
