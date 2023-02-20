using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseUnit : MonoBehaviour
{
    public int unitID = -1;
    public Tile OccupiedTile;
    public ScriptableUnit unitClass;
    public List<EffectList> effects = new List<EffectList>();

    public int cooldown = 0;

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

    private int _maxHealth = -1;
    private int _health = -1;
    private int _block = 0;
    private int _attackDamage = -1;
    private int _moveDistance = -1;
    private int _maxAttacks = -1;
    private int _attacksMade = 0;
    private int _attackRange = -1;
    private bool _isRanged = false;
    private string _unitDescription;
    private Faction _faction;
    private string _UnitName;
    private string _standActionName;

    public void OnEnable()
    {
        _health = _maxHealth;
        initUnit();
    }

    public void initUnit()
    {
        _maxHealth = unitClass.maxHealth;
        _health = unitClass.health;
        _attackDamage = unitClass.damage;
        _moveDistance = unitClass.movementRange;
        _maxAttacks = unitClass.attacks;
        _attacksMade = unitClass.attacks;
        _attackRange = unitClass.attackRange;
        _isRanged = unitClass.isRanged;
        _unitDescription = unitClass.unitDescription;
        _faction = unitClass.faction;
        _UnitName = unitClass.name;
        _standActionName = unitClass.ability.abilityName;
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
        if (_faction == Faction.Hero)
            unitClass.health -= dmg;    // Applied also to unit class (so next fight is started with health from last fight)
        
        
        // BUG: often is null -> crash
        // this.GetComponent<HitEffect>().StartCoroutine(this.GetComponent<HitEffect>().hitFlash());
        // DamagePopup.Create(transform.position, dmg, 0);
        MenuManager.Instance.UpdateHealthBar(this);
    }

    public int MaxHealth { get => _maxHealth; }
    public int Health { get => _health; }
    public int AttackDamage { get => _attackDamage; }
    public void changeDamage(int amount) { _attackDamage = _attackDamage + amount; }
    public void changeBlock(int amount = 0) { _block = _block + amount; if (amount == 0) { _block = 0; } }
    public int MoveDistance { get => _moveDistance; }
    public int AttackRange { get => _attackRange; }
    public Faction Faction { get => _faction; }
    public int MaxAttacks { get => _maxAttacks; }
    public string UnitName { get => _UnitName; }
    public string UnitDescription { get => _unitDescription; }

    public string StandActionName
    {
        get { return _standActionName;}
        set { _standActionName = value; }
        
    }
    public int AttacksMade
    {
        get { return _attacksMade;}
        set { _attacksMade = value; }
    }
    public bool IsRanged { get => _isRanged; }
    //public bool usedAction = false;
    public bool standAction = false;

    public virtual void AttackTarget(BaseUnit target)
    {
        AttacksMade--;
        target.TakeDamage(_attackDamage);
        if (_faction == Faction.Hero)
            MenuManager.Instance.updateAttacks(this);
        
        UnitManager.Instance.CheckAttackedUnit(target);
    }

    public void SpecialMove(Tile target)
    {
        unitClass.ability.Activate(this);
        cooldown = unitClass.ability.cooldown;
        MenuManager.Instance.updataAbility((BaseHero)this);
    }
    
    public void updateCooldowns()
    {
        _attacksMade = _maxAttacks;

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
