using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fighter : BaseHero
{
    public void Reset()
    {
        Faction = Faction.Hero;
        UnitName = "Fighter";
        _health = 150;
    }
}
