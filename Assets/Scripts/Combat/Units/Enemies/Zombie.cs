using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zombie : BaseEnemy
{
    public void Reset()
    {
        Faction = Faction.Enemy;
        UnitName = "Zombie";
        _health = 80;
    }
}
