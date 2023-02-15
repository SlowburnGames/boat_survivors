using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wizard : BaseHero
{
    public void Reset()
    {
        faction = Faction.Hero;
        UnitName = "Wizard";
        _maxHealth = 2;
        _attackRange = 4;
        _isRanged = true;
    }
}
