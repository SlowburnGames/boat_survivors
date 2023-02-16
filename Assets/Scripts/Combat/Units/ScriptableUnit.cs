using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "New Unit", menuName = "Scriptable Unit")]


public class ScriptableUnit : ScriptableObject
{
    // public Faction Faction;
    public BaseUnit UnitPrefab;
    public Faction faction;
    public string unitName;
    public string unitDescription;
    public bool isRanged;
    public int attacks;
    public int damage;
    public int movementRange;
    public int attackRange;
    public int health;
    public Sprite sprite;
    public Ability ability;

}

public enum Faction
{
    Hero = 0,
    Enemy = 1
}