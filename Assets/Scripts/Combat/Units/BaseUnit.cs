using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseUnit : MonoBehaviour
{
    public Tile OccupiedTile;
    public Faction faction;
    public string UnitName;
    public ScriptableUnit unitClass;
    public List<EffectList> effects = new List<EffectList>();

    public int cooldown = 0;
    public int attacksMade = 0;

    public bool invisible = false;
    public bool abilityReady = true;

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
    [SerializeField] protected int _block = 0;
    [SerializeField] protected int _attackDamage = 1;
    [SerializeField] protected int _moveDistance = 3;
    [SerializeField] protected int _maxAttacks = 3;
    [SerializeField] protected int _attackRange = 1;
    [SerializeField] protected bool _isRanged = false;

    [SerializeField] public string unitDescription;

    public void OnEnable()
    {
        _health = _maxHealth;
        initUnit();
    }

    public void initUnit()
    {
        _maxHealth = unitClass.health;
        _health = unitClass.health;
        _attackDamage = unitClass.damage;
        _moveDistance = unitClass.movementRange;
        _maxAttacks = unitClass.attacks;
        attacksMade = unitClass.attacks;
        _attackRange = unitClass.attackRange;
        _isRanged = unitClass.isRanged;
        unitDescription = unitClass.unitDescription;
        faction = unitClass.faction;
        UnitName = unitClass.name;
        standActionName = unitClass.ability.abilityName;
    }

    public void TakeDamage(int dmg)
    {
        if(_block != 0)
        {
            int temp = dmg;
            dmg = dmg - _block;
            _block = _block - temp;
        }
        _health -= dmg;
        this.GetComponent<HitEffect>().StartCoroutine(this.GetComponent<HitEffect>().hitFlash());
        DamagePopup.Create(transform.position, dmg, 0);
        MenuManager.Instance.UpdateHealthBar(this);
    }

    public int MaxHealth { get => _maxHealth; }
    public int Health { get => _health; }
    public int AttackDamage { get => _attackDamage; }
    public void changeDamage(int amount) { _attackDamage = _attackDamage + amount; }
    public void changeBlock(int amount = 0) { _block = _block + amount; if (amount == 0) { _block = 0; } }
    public int MoveDistance { get => _moveDistance; }
    public int AttackRange { get => _attackRange; }
    public int MaxAttacks { get => _maxAttacks; }
    public bool IsRanged { get => _isRanged; }
    //public bool usedAction = false;
    public bool standAction = false;

    public string standActionName;
    public virtual void AttackTarget(BaseUnit target)
    {
        target.TakeDamage(_attackDamage);
    }

    public void SpecialMove(Tile target)
    {
        unitClass.ability.Activate(this);
        cooldown = unitClass.ability.cooldown;
        MenuManager.Instance.updataAbility((BaseHero)this);
    }
    
    public void updateCooldowns()
    {
        attacksMade = _maxAttacks;

        if(cooldown >0)
        {
            cooldown--;
        }
        if(cooldown == 0 && !abilityReady)
        {
            abilityReady = true;
        }
        if(effects.Count <= 0)
        {
            return;
        }
        for (int i = 0; i < effects.Count; i++)
        {
            if (effects[i].turnsLeft > 0)
            {
                effects[i].turnsLeft--;
            }
            if(Effect.Instance.EditEffect(effects[i], this))
            {
                i++;
            }
        }
        //foreach (var eff in effects)
        //{
        //    if(eff.turnsLeft > 0)
        //    {
        //        eff.turnsLeft--;
        //    }
        //    Effect.Instance.EditEffect(eff, this);
        //}
        
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
