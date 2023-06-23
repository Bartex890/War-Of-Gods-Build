using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[System.Serializable]
[CreateAssetMenu(fileName = "newUnit", menuName = "Units/Unit")]
public class Unit : MapObject
{
    public enum UnitType
    {
        warrior,
        archer,
        worshipper,
        god,
        flying,
        werewolf,
        basilisk,
        ladyMidday,
        drowned
    }
    [SerializeField]
    private UnitType _type;
    private Gods _faction;
    private int _hp;
    [SerializeField]
    private int _baseHP;
    [SerializeField]
    private float _baseHPmultiplier;
    [SerializeField]
    private int _attackStrength;
    private int _baseAttackStrength;
    [SerializeField]
    private int _multiplierAttackStrength;

    private float _damageTakenMultiplier = 1;

    [HideInInspector]
    public int attackRange;
    [HideInInspector]
    public int moveRange;
    [SerializeField]
    private int _baseRange;
    [SerializeField]
    private int _baseRangeAttack;

    [HideInInspector]
    public bool occupation;
    [HideInInspector]
    public bool isOccupation;

    [SerializeField]
    private List<StatusEffect> _onAttackStatusEffects = new List<StatusEffect>();

    private List<StatusEffect> _currentStatusEffects = new List<StatusEffect>();


    public int onAttackTimer;
    public bool isTarget;

    public int numberOfUnits;

    public Animator animator;

    [HideInInspector]
    public Dictionary<string, GameObject> statusEffectDisplays = new Dictionary<string, GameObject>();




    public Unit(string name, int hp, int attackStrength, int attackRange, int moveRange, int multiplierAttackStrength, int teamID, string unitID)
    {
        this.name = name;
        this._baseHP = hp;
        this._hp = _baseHP;
        this._attackStrength = attackStrength;
        this._baseRangeAttack = attackRange;
        this._baseRange = moveRange;
        this.teamID = teamID;
        this.ID = unitID;
        this.occupation = false;
        this.isOccupation = false;

        this.actions = new string[] { "info" };

        this._multiplierAttackStrength = multiplierAttackStrength;
        processRound();
    }

    public Unit Init()
    {
        numberOfUnits = 1;
        _hp = _baseHP;
        resetRange();
        resetRangeAttack();
        onAttackTimer = 0;
        isTarget = true;
        if (teamID >= 0)
        {
            _faction = new Gods { gods = GameManager.Instance.GetTeam(teamID).GetFaction().gods };
            if (_faction.gods == Gods.Factions.perun && _type == UnitType.warrior)
            {
                _attackStrength += _faction.GetGodWarriorAttack();
            }
            GameManager.Instance.GetTeam(teamID).GetFaction();
        }

        _currentStatusEffects = new List<StatusEffect>();
        _baseAttackStrength = _attackStrength;
        _damageTakenMultiplier = 1;

        statusEffectDisplays = new Dictionary<string, GameObject>();
        return this;
    }
    public void processRound()
    {
        resetRange();
        resetRangeAttack();
        if (GameManager.Instance.GetCurrentTeam() == teamID || (teamID < 0 && GameManager.Instance.GetCurrentTeam() == 0))
        {
            _damageTakenMultiplier = 1;

            for (int i = _currentStatusEffects.Count - 1; i >= 0; i--)
            {
                StatusEffect se = _currentStatusEffects[i];

                if (se.duration > 0)
                {
                    se.duration -= 1;
                }

                if (se.ProcessStatusEffect(this) == false)
                {
                    return;
                }

                if (se.duration == 0)
                {
                    se.EraseStatusEffect(this);
                    _currentStatusEffects.RemoveAt(i);
                }
            }

            for (int i = _onAttackStatusEffects.Count - 1; i >= 0; i--)
            {
                StatusEffect se = _onAttackStatusEffects[i];

                if (se.duration > 0)
                {
                    se.duration -= 1;
                }
                if (se.duration == 0)
                {
                    if (se.animationWhenApplied != "")
                    {
                        TryDestroyEffectAnimation(se.animationWhenApplying);
                    }
                    _onAttackStatusEffects.RemoveAt(i);
                }
            }
        }
    }

    public void takeDamage(int amount)
    {
        amount = (int)(amount * _damageTakenMultiplier);
        
        FXAnimationManager.PlayAnimationAtPosition("HitMark", position);
        if (amount >= _hp)
        {
            
            numberOfUnits--;
            amount -= _hp;
            _hp = _baseHP;
            if (teamID < 0)
            {
                //Earning faith points
                GameManager.Instance.GetCurrentTeamObj().AddFaithPoints(1);
                GameManager.Instance.RefreshResourceDisplays();
            }
            while (amount >= _baseHP)
            {
                numberOfUnits--;
                amount -= _baseHP;
                if (teamID < 0)
                {
                    //Earning faith points
                    GameManager.Instance.GetCurrentTeamObj().AddFaithPoints(1);
                    GameManager.Instance.RefreshResourceDisplays();
                }
            }
            if (amount < _baseHP && amount > 0)
            {
                _hp -= amount;
            }
        }
        else
        {
            _hp -= amount;
        }

    }
    public int getHP() { return _hp; }
    public int getBaseHP() { return (int)Mathf.Ceil(_baseHP * _baseHPmultiplier); }
    public int getBaseStrengthAttack() { return _baseAttackStrength; }
    public int getBaseMoveRange() { return _baseRange; }
    public void resetRange() { moveRange = (int)(_baseRange * EventModifier.Instance.percentBuffRangeMove); }
    public void resetRangeAttack() { attackRange = _baseRangeAttack; }
    public int getAttackStrength()
    {
        if (_type == UnitType.drowned && new string[] { "r", "o" }.Contains(MapManager.Instance.map.tiles[position.x, position.y]))
        {
            return _attackStrength * 2;
        }
        return _attackStrength;
    }
    public int getAttackStrengthBuilding(bool isUnit)
    {
        if (isUnit)
        {
            return (_attackStrength * _multiplierAttackStrength) / 2;
        }
        return _attackStrength * _multiplierAttackStrength;
    }
    public void applyStatusEffect(StatusEffect statusEffect)
    {
        if (teamID >= 0 && _faction.gods == Gods.Factions.marzanna) return;
        if (statusEffect == null) Debug.LogError("statusEffect is null :/");

        StatusEffect se = Instantiate(statusEffect);


        se.ApplyStatusEffect(this);
        _currentStatusEffects.Add(se);
    }
    public void applyStatusEffects(List<StatusEffect> statusEffects)
    {
        if (teamID >= 0 && _faction.gods == Gods.Factions.marzanna) return;

        foreach (StatusEffect se in statusEffects)
        {
            applyStatusEffect(se);
        }
    }
    public void multiplyRange(float multiplier)
    {
        moveRange = (int)(moveRange * multiplier);
    }

    public void multiplyDamage(float multiplier)
    {
        _attackStrength = (int)(_attackStrength * multiplier);
    }

    public List<StatusEffect> getAppliedStatusEffects()
    {
        return _onAttackStatusEffects;
    }

    public void addAppliedStatusEffect(StatusEffect statusEffect)
    {
        _onAttackStatusEffects.Add(Instantiate(statusEffect));
        TryAddStatusEffectAnimation(statusEffect.animationWhenApplying);

    }

    public Gods GetFaction()
    {
        return _faction;
    }

    public int GetAmountOfStatusEffects()
    {
        return _currentStatusEffects.Count;
    }

    //public void Freeze(int timer)
    //{
    //    attackRange = 0;
    //    moveRange = 0;
    //    onAttackTimer = timer;
    //}
    public void SetIsTarget(bool target = false)
    {
        isTarget = target;
    }
    public void SetUpgradeUnit()
    {
        if (_faction.gods == Gods.Factions.swarog)
        {
            switch (_type)
            {
                case UnitType.archer: _baseHP += _faction.GetGodArcherHP(); break;
                case UnitType.warrior: _baseHP += _faction.GetGodWarriorHP(); break;
                case UnitType.worshipper: _baseHP += _faction.GetGodWorshipperHP(); break;
            }

        }
    }

    public UnitType GetUnitType()
    {
        return _type;
    }

    public void SetHP(int amount)
    {
        _hp = amount;
    }

    public Sprite GetSprite()
    {
        switch (_type)
        {
            case UnitType.archer: return UnitsSprites.Instance.archerSprite;
            case UnitType.warrior:
                switch (name)
                {
                    case "Cavalry": return UnitsSprites.Instance.cavarlySprite;
                    case "Warrior": return UnitsSprites.Instance.warriorSprite;
                    case "Bear": return UnitsSprites.Instance.bearSprite;
                    case "Bies": return UnitsSprites.Instance.biesSprite;
                    case "Leshy": return UnitsSprites.Instance.LeshySprite;
                    case "Wolf": return UnitsSprites.Instance.wolfSprite;
                }
                return UnitsSprites.Instance.warriorSprite;
            case UnitType.worshipper: return UnitsSprites.Instance.worshipperSprite;
            case UnitType.god:
                switch (name)
                {
                    case "Devana": return UnitsSprites.Instance.dziewannaSprite;
                    case "Marzanna": return UnitsSprites.Instance.marzannaSprite;
                    case "Perun": return UnitsSprites.Instance.perunSprite;
                    case "Swaróg": return UnitsSprites.Instance.swarogSprite;
                    case "Œwiêtowid": return UnitsSprites.Instance.swietowidSprite;
                }
                return UnitsSprites.Instance.perunSprite;
            case UnitType.flying:
                switch (name)
                {
                    case "Griffin": return UnitsSprites.Instance.gryffinSprite;
                    case "Rusalka": return UnitsSprites.Instance.RusalkaSprite;
                    case "Utopiec": return UnitsSprites.Instance.utopiecSprite;
                    case "Witch": return UnitsSprites.Instance.witchSprite;
                }
                return UnitsSprites.Instance.gryffinSprite;
            case UnitType.werewolf: return UnitsSprites.Instance.werewolfSprite;
            case UnitType.basilisk: return UnitsSprites.Instance.basilicsSprite;
            case UnitType.ladyMidday: return UnitsSprites.Instance.LadyMiddaySprite;
            default: return null;
        }
    }

    public void SetAttackStrength(int amount)
    {
        _attackStrength = amount;
    }
    public void SetMoveRange(int amount)
    {
        moveRange = amount;
    }
    public void SetBaseHPmultiplier(float amount)
    {
        _baseHPmultiplier = amount;
    }

    public List<StatusEffect> GetStatusEffects()
    {
        return _currentStatusEffects;
    }

    public void TryAddStatusEffectAnimation(string animationName)
    {
        if (!statusEffectDisplays.ContainsKey(animationName))
        {
            MapManager.Instance.AddAnimationToUnit(position, animationName);
        }
    }

    public void TryDestroyEffectAnimation(string animationName)
    {
        if (statusEffectDisplays.ContainsKey(animationName))
        {
            Destroy(statusEffectDisplays.GetValueOrDefault(animationName));
            statusEffectDisplays.Remove(animationName);
        }
    }

    public void multiplyDamageTaken(float multiplier)
    {
        _damageTakenMultiplier *= multiplier;

        Debug.Log(_damageTakenMultiplier);
    }

    public void OnDeath()
    {
       
    }
}
