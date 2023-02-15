using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : BaseEnemy
{
    public void Reset()
    {
        faction = Faction.Enemy;
        UnitName = "Monster";
        _maxHealth = 4;
    }
}
