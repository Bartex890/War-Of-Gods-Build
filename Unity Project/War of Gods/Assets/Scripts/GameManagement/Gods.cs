using System;
using UnityEngine;

[Serializable]
public class Gods
{
    public enum Factions
    {
        perun,
        swarog,
        marzanna,
        dziewanna,
        swietowid,
        empty
    }
    public Factions gods;

    public int GetGodBuilding()
    {
        switch (gods)
        {
            case Factions.perun: return ModifyBuildingsGods.Instance.modifyBuildingsPerun;
            case Factions.swarog: return ModifyBuildingsGods.Instance.modifyBuildingsSwarog;
            case Factions.marzanna: return ModifyBuildingsGods.Instance.modifyBuildingsMarzanna;
            case Factions.dziewanna: return ModifyBuildingsGods.Instance.modifyBuildingsDziewanna;
            case Factions.swietowid: return ModifyBuildingsGods.Instance.modifyBuildingsSwietowid;
            default: return 0;
        }
    }

    public float GetGodWoodModifier()
    {
        switch (gods)
        {
            case Factions.perun: return ModifyBuildingsGods.Instance.perunWoodModifier;
            case Factions.swarog: return ModifyBuildingsGods.Instance.swarogWoodModifier;
            case Factions.marzanna: return ModifyBuildingsGods.Instance.marzannaWoodModifier;
            case Factions.dziewanna: return ModifyBuildingsGods.Instance.dziewannaWoodModifier;
            case Factions.swietowid: return ModifyBuildingsGods.Instance.swietowidWoodModifier;
            default: return 0;
        }
    }
    public float GetGodFoodModifier()
    {
        switch (gods)
        {
            case Factions.perun: return ModifyBuildingsGods.Instance.perunFoodModifier;
            case Factions.swarog: return ModifyBuildingsGods.Instance.swarogFoodModifier;
            case Factions.marzanna: return ModifyBuildingsGods.Instance.marzannaFoodModifier;
            case Factions.dziewanna: return ModifyBuildingsGods.Instance.dziewannaFoodModifier;
            case Factions.swietowid: return ModifyBuildingsGods.Instance.swietowidFoodModifier;
            default: return 0;
        }
    }

    public int GetGodWarriorAttack()
    {
        switch (gods)
        {
            case Factions.perun: return ModifyUnitsGods.Instance.modifyWarriorPerun;
            case Factions.swarog: return ModifyUnitsGods.Instance.modifyWarriorSwarog;
            case Factions.marzanna: return ModifyUnitsGods.Instance.modifyWarriorMarzanna;
            case Factions.dziewanna: return ModifyUnitsGods.Instance.modifyWarriorDziewanna;
            case Factions.swietowid: return ModifyUnitsGods.Instance.modifyArcherSwietowid;
            default: return 0;
        }
    }
    public int GetGodArcherAttack()
    {
        switch (gods)
        {
            case Factions.perun: return ModifyUnitsGods.Instance.modifyArcherPerun;
            case Factions.swarog: return ModifyUnitsGods.Instance.modifyArcherSwarog;
            case Factions.marzanna: return ModifyUnitsGods.Instance.modifyWarriorMarzanna;
            case Factions.dziewanna: return ModifyUnitsGods.Instance.modifyWarriorDziewanna;
            case Factions.swietowid: return ModifyUnitsGods.Instance.modifyArcherSwietowid;
            default: return 0;
        }
    }
    public int GetGodWorshipperAttack()
    {
        switch (gods)
        {
            case Factions.perun: return ModifyUnitsGods.Instance.modifyWorshipperPerun;
            case Factions.swarog: return ModifyUnitsGods.Instance.modifyWorshipperSwarog;
            case Factions.marzanna: return ModifyUnitsGods.Instance.modifyWorshipperMarzanna;
            case Factions.dziewanna: return ModifyUnitsGods.Instance.modifyWorshipperDziewanna;
            case Factions.swietowid: return ModifyUnitsGods.Instance.modifyWorshipperSwietowid;
            default: return 0;
        }
    }
    public int GetGodWarriorHP()
    {
        switch (gods)
        {
            case Factions.perun: return ModifyUnitsGods.Instance.modifyHPWarriorPerun;
            case Factions.swarog: return ModifyUnitsGods.Instance.modifyHPWarriorSwarog;
            case Factions.marzanna: return ModifyUnitsGods.Instance.modifyHPWarriorMarzanna;
            case Factions.dziewanna: return ModifyUnitsGods.Instance.modifyHPWarriorDziewanna;
            case Factions.swietowid: return ModifyUnitsGods.Instance.modifyHPArcherSwietowid;
            default: return 0;
        }
    }
    public int GetGodArcherHP()
    {
        switch (gods)
        {
            case Factions.perun: return ModifyUnitsGods.Instance.modifyHPArcherPerun;
            case Factions.swarog: return ModifyUnitsGods.Instance.modifyHPArcherSwarog;
            case Factions.marzanna: return ModifyUnitsGods.Instance.modifyHPWarriorMarzanna;
            case Factions.dziewanna: return ModifyUnitsGods.Instance.modifyHPWarriorDziewanna;
            case Factions.swietowid: return ModifyUnitsGods.Instance.modifyHPArcherSwietowid;
            default: return 0;
        }
    }
    public int GetGodWorshipperHP()
    {
        switch (gods)
        {
            case Factions.perun: return ModifyUnitsGods.Instance.modifyHPWorshipperPerun;
            case Factions.swarog: return ModifyUnitsGods.Instance.modifyHPWorshipperSwarog;
            case Factions.marzanna: return ModifyUnitsGods.Instance.modifyHPWorshipperMarzanna;
            case Factions.dziewanna: return ModifyUnitsGods.Instance.modifyHPWorshipperDziewanna;
            case Factions.swietowid: return ModifyUnitsGods.Instance.modifyHPWorshipperSwietowid;
            default: return 0;
        }
    }

    public Sprite GetPortait()
    {
        switch (gods)
        {
            case Factions.perun: return PortraitsGods.Instance.perunPortrait;
            case Factions.swarog: return PortraitsGods.Instance.swarogPortrait;
            case Factions.marzanna: return PortraitsGods.Instance.marzannaPortrait;
            case Factions.dziewanna: return PortraitsGods.Instance.dziewannaPortrait;
            case Factions.swietowid: return PortraitsGods.Instance.swietowidPortrait;
            default: return null;
        }
    }
}
