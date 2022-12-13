using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rouge : BaseHero
{
    public void Reset()
    {
        Faction = Faction.Hero;
        UnitName = "Rouge";
        _maxHealth = 50;
        _moveDistance = 5;
    }
}
