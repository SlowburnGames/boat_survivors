using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseUnit : MonoBehaviour
{
    public Tile OccupiedTile;
    public Faction Faction;
    public string UnitName;

    public bool invisible = false;

    public bool Invisible
    {
        get {return invisible;}
        set {
            invisible = value;
            Color color = GetComponent<SpriteRenderer>().color;
            if(value)
            {
                color.a = 0.5f;
            }
            else
            {
                color.a = 1f;
            }
            GetComponent<SpriteRenderer>().color = color;
        }
    }

    [SerializeField] protected int _maxHealth = 2;
    [SerializeField] protected int _health;
    [SerializeField] protected int _attackDamage = 1;
    [SerializeField] protected int _moveDistance = 3;
    [SerializeField] protected int _attackRange = 1;
    [SerializeField] protected bool _isRanged = false;

    [SerializeField]public string unitDescription;

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

    public virtual void AttackTarget(BaseUnit target)
    {
        target.Attack(_attackDamage);
    }

    public virtual void SpecialMove(Tile target)
    {

    }
    

    private void Update() {
        checkInvisible();
    }

    void checkInvisible()
    {
        Color color = GetComponent<SpriteRenderer>().color;
        if(invisible)
        {
            color.a = 0.5f;
        }
        else
        {
            color.a = 1f;
        }
        GetComponent<SpriteRenderer>().color = color;
    }
    // public int MovementRange;
}
