using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseUnit : MonoBehaviour
{
    public Tile OccupiedTile;
    public Faction Faction;
    public string UnitName;

    [SerializeField] protected int _maxHealth = 100;
    [SerializeField] protected int _health;
    [SerializeField] protected int _attackDamage = 20;
    [SerializeField] protected int _moveDistance = 3;
    [SerializeField] protected int _attackRange = 1;
    [SerializeField] protected bool _isRanged = false;

    public void OnEnable()
    {
        _health = _maxHealth;
    }

    public void Attack(int dmg)
    {
        _health -= dmg;
        MenuManager.Instance.UpdateHealthBar(this);
    }

    public int MaxHealth { get => _maxHealth; }
    public int Health { get => _health; }
    public int AttackDamage { get => _attackDamage; }
    public int MoveDistance { get => _moveDistance; }
    public int AttackRange { get => _attackRange; }
    public bool IsRanged { get => _isRanged; }
    
}
