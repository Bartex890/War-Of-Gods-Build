using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SavedGame
{
    public string gameName;
    public string mapName;

    public int currentRound;
    public int currentTeam;

    public SerializableTeam[] teams;

    public SerializableUnit[] units;
    public SerializableBuilding[] buildings;
}