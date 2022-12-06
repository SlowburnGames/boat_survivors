using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : BaseEnemy
{
    public void Reset()
    {
        Faction = Faction.Enemy;
        UnitName = "Monster";
        _health = 150;
    }
}
