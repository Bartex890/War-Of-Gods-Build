using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using Unity.VisualScripting;
using UnityEngine;

public class LocalSaveUtility : MonoBehaviour
{
    private static LocalSaveUtility _instance;
    public static LocalSaveUtility Instance
    {
        get
        {
            if (_instance == null)
            {
                Debug.LogError("LocalSaveUtility is null");
            }
            return _instance;
        }
    }

    private void Awake()
    {
        _instance = this;
    }

    public void SaveTheGame()
    {
        SavedGame save = new SavedGame();

        save.gameName = SaveSettings.Instance.GetGameName();
        save.mapName = SaveSettings.Instance.GetMapName();

        save.currentRound = GameManager.Instance.GetCurrentRound();
        save.currentTeam = GameManager.Instance.currentTeam;

        save.teams = TeamArrayToSerializableTeamArray(GameManager.Instance.GetAllTeams().ToArray());

        save.units = UnitArrayToSerializableUnitArray(MapManager.Instance.map.getAllUnits().Values.ToArray());
        save.buildings = BuildingArrayToSerializableBuildingArray(MapManager.Instance.map.getAllBuildings().Values.ToArray());

        SaveManager.Save(save, save.gameName);
    }

    public SerializableUnit[] UnitArrayToSerializableUnitArray(Unit[] input)
    {
        SerializableUnit[] output = new SerializableUnit[input.Length];

        for (int i = 0; i < input.Length; i++)
        {
            output[i] = UnitToSerializableUnit(input[i]);
        }

        return output;
    }

    public SerializableBuilding[] BuildingArrayToSerializableBuildingArray(Building[] input)
    {
        SerializableBuilding[] output = new SerializableBuilding[input.Length];

        for (int i = 0; i < input.Length; i++)
        {
            output[i] = BuildingToSerializableBuilding(input[i]);
        }

        return output;
    }

    public SerializableTeam[] TeamArrayToSerializableTeamArray(Team[] input)
    {
        SerializableTeam[] output = new SerializableTeam[input.Length];

        for (int i = 0; i < input.Length; i++)
        {
            output[i] = TeamToSerializableTeam(input[i]);
        }

        return output;
    }

    public SerializableUnit UnitToSerializableUnit(Unit unit)
    {
        SerializableUnit output = new SerializableUnit();

        output.id = unit.ID;
        output.teamID = unit.teamID;
        output.posX = unit.position.x; output.posY = unit.position.y;
        output.hpLeft = unit.getHP();
        output.numberOfUnits = unit.numberOfUnits;
        output.statusEffects = StatusEffectArrayToStatusEffectAndTimeArray(unit.GetStatusEffects().ToArray());
        output.appliedEffects = StatusEffectArrayToStatusEffectAndTimeArray(unit.getAppliedStatusEffects().ToArray());

        if (unit.GetUnitType() == Unit.UnitType.werewolf)
        {
            output.id = "werewolf";
        }

        return output;
    }

    public SerializableUnit.StatusEffectAndTime[] StatusEffectArrayToStatusEffectAndTimeArray(StatusEffect[] input)
    {
        SerializableUnit.StatusEffectAndTime[] output = new SerializableUnit.StatusEffectAndTime[input.Length];

        for (int i = 0; i < input.Length; i++)
        {
            output[i] = new SerializableUnit.StatusEffectAndTime();
            output[i].FromStatusEffect(input[i]);
        }

        return output;
    }

    public SerializableBuilding BuildingToSerializableBuilding(Building building)
    {
        SerializableBuilding output = new SerializableBuilding();

        output.id = building.ID;
        output.teamID = building.teamID;
        output.posX = building.position.x; output.posY = building.position.y;
        output.pounding = building.pounding;
        output.actions = building.actions;

        return output;
    }

    public SerializableTeam TeamToSerializableTeam(Team team)
    {
        SerializableTeam output = new SerializableTeam();

        /*
        public float lastCameraX, lastCameraY;
        public Factions god;
        public int amountWood;
        public int amountGold;
        public int amountFood;
        public int amountFaithPoints;
        public bool[,] visitedTiles;
        public bool isDefeat;
        */

        output.lastCameraX = team.GetLastCameraPosition().x;
        output.lastCameraY = team.GetLastCameraPosition().y;
        output.god = team.GetFaction().gods;
        output.amountWood = team.GetAmountWood();
        output.amountGold = team.GetAmountGold();
        output.amountFood = team.GetAmountFood();
        output.amountFaithPoints = team.GetAmountFaithPoints();
        output.visitedTiles = team.visitedTiles;
        output.isDefeat = team.isDefeat;

        return output;
    }
}
