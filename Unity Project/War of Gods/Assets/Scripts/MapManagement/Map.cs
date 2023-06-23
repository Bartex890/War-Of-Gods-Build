using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
//using static UnityEditor.PlayerSettings;

public class Map
{
    public string[,] tiles;

    private Dictionary<Vector2Int, Building> _buildings = new Dictionary<Vector2Int, Building>();
    private Dictionary<Vector2Int, Unit> _units = new Dictionary<Vector2Int, Unit>();

    private int _width;
    private int _height;

    public Map(int width, int height)
    {
        _width = width;
        _height = height;

        tiles = new string[width, height];
    }

    public void RemoveUnit(Vector2Int pos)
    {
        _units.Remove(pos);
    }

    public void AddBuilding(Vector2Int pos, Building building)
    {
        _buildings.Add(pos, building);
    }
    public void RemoveBuilding(Vector2Int pos)
    {
        _buildings.Remove(pos);
    }

    public void AddUnit(Vector2Int pos, Unit unit)
    {
        _units.Add(pos, unit);
        unit.position = pos;
    }

    public Building getBuilding(Vector2Int pos)
    {
        _buildings.TryGetValue(pos, out Building o);
        return o;
    }
    public bool isBuild(Vector2Int pos)
    {
        if (getBuilding(pos) == null)
        return true;
        return false;
    }
    public bool isTileNotToGo(Vector2Int pos, Unit unit)
    {
        if (pos.x < 0 || pos.x >= tiles.GetLength(0) || pos.y < 0 || pos.y >= tiles.GetLength(1))
            return true;

        if (FogOfWarManager.Instance.fogOfWar.GetPixel(pos.x, pos.y).r > 0.5)
            return true;

        if (unit.GetUnitType() == Unit.UnitType.flying)
        {
            if (new string[] { "o" }.Contains(tiles[pos.x, pos.y]))
                return true;
            return false;
        }

        if (unit.GetUnitType() == Unit.UnitType.drowned)
        {
            if (new string[] { "g", "l", "o" }.Contains(tiles[pos.x, pos.y]))
                return true;
            return false;
        }

        if (new string[] { "g", "r", "l", "o" }.Contains(tiles[pos.x,pos.y]))
            return true;
        return false;
    }
    public Unit getUnit(Vector2Int pos)
    {
        _units.TryGetValue(pos, out Unit o);
        return o;
    }
    public void MoveUnit(Vector2Int from, Vector2Int to, int range)
    {
        getUnit(from).moveRange =range;
        AddUnit(to, getUnit(from));
        RemoveUnit(from);
    }
    public void OccupationUnit(Vector2Int pos)
    {
        getUnit(pos).moveRange = 0;
        getBuilding(pos).SwitchTeam(getUnit(pos).teamID);
        //tutaj mo�na odejmowa� niby surowce
    }
    public void IsOcupationUnit()
    {
        Dictionary<Vector2Int,Unit>.ValueCollection temp = _units.Values;
        foreach (Unit unit in temp)
        {
            if (unit.occupation) unit.isOccupation = true;
        }
    }
    public void AttackUnit(Vector2Int attackUnit, Vector2Int attacker, bool isBuilding)
    {
        Unit unit = getUnit(attackUnit);
        Unit attackerUnit = getUnit(attacker);

        //unique animation fx
        switch (attackerUnit.ID)
        {
            case "perun":
                FXAnimationManager.PlayAnimationAtPosition("Thunder", attackUnit);
                break;
        }

        if (isBuilding)
        {

            unit.takeDamage((unit.getAttackStrength()/2)* attackerUnit.numberOfUnits);

        }
        else
        {
            
            if (unit is Monster)
            {
                if (attackerUnit.ID == "dziewanna") unit.takeDamage(unit.getAttackStrength()*3* attackerUnit.numberOfUnits);
                if (unit.GetUnitType()==Unit.UnitType.basilisk) attackerUnit.applyStatusEffect(StatusEffectDataHolder.Instance.statusEffectData.GetValueOrDefault("stunned"));
            } else
                unit.takeDamage(unit.getAttackStrength() * attackerUnit.numberOfUnits);
        }

        

        Debug.Log("HP zosta�o: " + unit.getHP() + " Ruch " + getUnit(attacker).moveRange + "Gracz ma jeszcze zasiegu ataku: " + getUnit(attacker).attackRange);
        if ((Mathf.Abs(attackUnit.x - attacker.x) + Mathf.Abs(attackUnit.y - attacker.y)) <= attackerUnit.attackRange && attackerUnit.attackRange!=0)
        {
            attackerUnit.attackRange = 0;
            if (getUnit(attackUnit)!=null && getUnit(attackUnit).attackRange >= Mathf.Abs(attackUnit.x - attacker.x) + Mathf.Abs(attackUnit.y - attacker.y))
            {
                if (getBuilding(attacker) != null)
                {
                    AttackUnit(attacker, attackUnit, true);
                }
                else
                {
                    AttackUnit(attacker, attackUnit, false);
                }
            }
        }
        else
        {
            attackerUnit.attackRange = 0;
        }
    }
    public void ResetMovementOptions()
    {
        for (int i = _units.Count - 1; i >= 0; i--)
        {
            var unit = _units.ToArray()[i];
            unit.Value.processRound();
        }
    }
    public void ResetIsTarget()
    {
        foreach (var unit in _units)
        {
            unit.Value.SetIsTarget(true);
        }
    }

    public Dictionary<Vector2Int, Unit> getAllUnits()
    {
        return _units;
    }

    public Dictionary<Vector2Int, Building> getAllBuildings()
    {
        return _buildings;
    }
}
