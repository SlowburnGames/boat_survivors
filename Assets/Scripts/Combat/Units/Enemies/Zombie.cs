using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zombie : BaseEnemy
{
    public void Reset()
    {
        faction = Faction.Enemy;
        UnitName = "Zombie";
        _maxHealth = 2;
    }
}
