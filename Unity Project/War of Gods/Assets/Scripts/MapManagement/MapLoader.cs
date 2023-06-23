using UnityEngine;
using System.IO;
using System.Collections.Generic;

public class MapLoader : MonoBehaviour
{
    public static Map LoadMap(string mapDir, bool ignoreMapObjects = false)
    {
        string data = File.ReadAllText(mapDir);

        string[,] tileStrings = CsvOperations.CsvToArray(data);

        Map output = new Map(tileStrings.GetLength(0), tileStrings.GetLength(1));

        for(int x = 0; x < tileStrings.GetLength(0); x++)
        {
            for (int y = 0; y < tileStrings.GetLength(1); y++)
            {
                string[] cell = tileStrings[x, y].Split(',');
                //GetTile
                if (cell[0] != null)
                {
                    output.tiles[x, y] = cell[0];
                }
                else
                {
                    output.tiles[x, y] = "";
                }

                if (ignoreMapObjects) continue;

                //TryPlaceBuilding
                try {
                    if (cell[1] != null)
                    {
                        string[] buildingSplit = cell[1].Split(':');
                        switch (buildingSplit[0])
                        {
                            case "b":
                                addBuildingToMap(output, "base", int.Parse(buildingSplit[1]), x, y); //Base building
                                GameManager.Instance.GetTeam(int.Parse(buildingSplit[1])-1).SetCameraPosition(new Vector3(x, -y, -10));
                                break;
                            case "f": 
                                addBuildingToMap(output, "farm", int.Parse(buildingSplit[1]), x, y); break; //Farm building
                            case "t":
                                addBuildingToMap(output, "lumber_mill", int.Parse(buildingSplit[1]), x, y); break; //Lumber Mill
                            case "chata_na_kurzej_stopie":
                                addBuildingToMap(output, "hut_on_a_chicken_foot", int.Parse(buildingSplit[1]), x, y); break; //Hut On a Chicken Foot
                            case "boski_oltarz":
                                addBuildingToMap(output, "godly_altar", int.Parse(buildingSplit[1]), x, y); break; //Godly Altar
                            case "ruiny":
                                addBuildingToMap(output, "ruins", int.Parse(buildingSplit[1]), x, y); break; //Ruins
                            case "swiety_gaj":
                                addBuildingToMap(output, "sacred_grove", int.Parse(buildingSplit[1]), x, y); break;

                        }
                    }
                } catch { }

                //TryPlaceUnit
                try
                {
                    if (cell[2] != null)
                    {
                        switch (tileStrings[x, y].Split(',')[2])
                        {
                            case "tu": output.AddUnit(new Vector2Int(x, y), new Unit("Test Unit", 5, 2, 1, 2, 10, 0, "test_unit")); break; //Test unit
                            case "tuu": output.AddUnit(new Vector2Int(x, y), new Unit("Test Unit", 5, 2, 1, 2, 10, 1, "test_unit")); break; //Test unit
                        }
                    }
                }
                catch { }
            }
        }

        return output;
    }

    private static void addBuildingToMap(Map map, string name, int team, int x, int y)
    {
        Building temp = Instantiate(BuildingDataHolder.GetBuildingData(name).building);
        temp.teamID = team - 1;
        temp.Init();
        temp.position = new Vector2Int(x, y);
        map.AddBuilding(temp.position, temp);
        //Debug.Log("Loaded " + temp.name);
    }

    public static Map LoadMapFromSave(SavedGame save)
    {
        Map output = LoadMap(Application.streamingAssetsPath + "/Maps/" + save.mapName + ".csv", true);

        foreach (SerializableUnit sUnit in save.units)
        {
            output.AddUnit(new Vector2Int(sUnit.posX, sUnit.posY), _GetUnitFromSerializableUnit(sUnit));
        }

        foreach (SerializableBuilding sBuilding in save.buildings)
        {
            output.AddBuilding(new Vector2Int(sBuilding.posX, sBuilding.posY), _GetBuildingFromSerializableBuilding(sBuilding));
        }

        return output;
    }

    public static void LoadStatusEffectsFromSave(SavedGame save)
    {
        foreach (SerializableUnit sUnit in save.units)
        {
            Unit unit = MapManager.Instance.map.getUnit(new Vector2Int(sUnit.posX, sUnit.posY));

            if (sUnit.statusEffects != null)
                foreach (SerializableUnit.StatusEffectAndTime seat in sUnit.statusEffects)
                {
                    unit.applyStatusEffect(_GetStatusEffectFromStatusEffectAndTime(seat));
                    Debug.Log(seat.id);
                }
            if (sUnit.appliedEffects != null)
                foreach (SerializableUnit.StatusEffectAndTime seat in sUnit.appliedEffects)
                {
                    unit.addAppliedStatusEffect(_GetStatusEffectFromStatusEffectAndTime(seat));

                }
        }
    }

    private static Unit _GetUnitFromSerializableUnit(SerializableUnit sUnit)
    {
        Unit output = Instantiate(UnitDataHolder.Instance.unitData.GetValueOrDefault(sUnit.id).unit);
        output.teamID = sUnit.teamID;
        output.Init();
        output.position = new Vector2Int(sUnit.posX, sUnit.posY);
        output.SetHP(sUnit.hpLeft);
        output.numberOfUnits = sUnit.numberOfUnits;

        /*if (sUnit.statusEffects != null)
            foreach(SerializableUnit.StatusEffectAndTime seat in sUnit.statusEffects)
            {
                output.applyStatusEffect(_GetStatusEffectFromStatusEffectAndTime(seat));
                Debug.Log(seat.id);
            }
        if (sUnit.appliedEffects != null)
            foreach (SerializableUnit.StatusEffectAndTime seat in sUnit.appliedEffects)
            {
                output.addAppliedStatusEffect(_GetStatusEffectFromStatusEffectAndTime(seat));
            }*/

        //Debug.Log("Loaded " + output.name);
        return output;
    }

    private static StatusEffect _GetStatusEffectFromStatusEffectAndTime(SerializableUnit.StatusEffectAndTime seat)
    {
        StatusEffect output = Instantiate(StatusEffectDataHolder.Instance.statusEffectData.GetValueOrDefault(seat.id));
        output.duration = seat.time;
        return output;
    }

    private static Building _GetBuildingFromSerializableBuilding(SerializableBuilding sBuilding)
    {
        Building output = Instantiate(BuildingDataHolder.GetBuildingData(sBuilding.id).building);
        output.teamID = sBuilding.teamID;
        output.Init();
        output.position = new Vector2Int(sBuilding.posX, sBuilding.posY);
        output.pounding = sBuilding.pounding;
        output.actions = sBuilding.actions;

        //Debug.Log("Loaded " + output.name);
        return output;
    }
}
