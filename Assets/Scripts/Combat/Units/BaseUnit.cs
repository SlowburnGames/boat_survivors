using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseUnit : MonoBehaviour
{
    public Tile OccupiedTile;
    public Faction Faction;
    public string UnitName;

    [SerializeField] protected int _health = 100;
    [SerializeField] protected int _attackDamage = 20;
    [SerializeField] protected int _moveDistance = 3;
    [SerializeField] protected int _attackRange = 1;
    [SerializeField] protected bool _isRanged = false;
    
    public void Attack(int dmg)
    {
        _health -= dmg;
    }

    public int Health { get => _health; }
    public int AttackDamage { get => _attackDamage; }
    public int MoveDistance { get => _moveDistance; }
    public int AttackRange { get => _attackRange; }
    public bool IsRanged { get => _isRanged; }
    
}
