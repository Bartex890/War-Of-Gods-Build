using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GodlyAltar", menuName = "Buildings/GodlyAltar")]
public class GodlyAltar : Building
{
    public GodlyAltar(string name, int teamID) : base(name, teamID, 11, "godly_altar")
    {
        GameManager.onTurnEnded += ProcessTurn;
        //actions = new string[] { "info", "spawn_witch" };
    }

    public override void ProcessTurn()
    {
        base.ProcessTurn();

        string godSpawnName = "spawn_marzanna";

        switch (GameManager.Instance.GetCurrentTeamObj().GetFaction().gods)
        {
            case Gods.Factions.marzanna: godSpawnName = "spawn_marzanna"; break;
            case Gods.Factions.dziewanna: godSpawnName = "spawn_dziewanna"; break;
            case Gods.Factions.swarog: godSpawnName = "spawn_swarog"; break;
            case Gods.Factions.swietowid: godSpawnName = "spawn_swietowid"; break;
            case Gods.Factions.perun: godSpawnName = "spawn_perun"; break;
        }

        actions = new string[] { "info", godSpawnName };
    }

    public override void Init()
    {
        GameManager.onTurnEnded += ProcessTurn;
    }
}
