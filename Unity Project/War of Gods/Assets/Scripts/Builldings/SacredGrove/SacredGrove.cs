using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SacredGrove", menuName = "Buildings/SacredGrove")]
public class SacredGrove : Building
{
    public SacredGrove(string name, int teamID) : base(name, teamID, 10, "sacred_grove")
    {
        GameManager.onTurnEnded += ProcessTurn;
        actions = new string[] { "info", "spawn_witch" };
    }

    public override void ProcessTurn()
    {
        base.ProcessTurn();
    }

    public override void Init()
    {
        string[] monsterIDs = new string[] { "spawn_leshy", "spawn_werewolf", "spawn_rusalka", "spawn_basilics", "spawn_ladyMidday","spawn_wolf","spawn_bear","spawn_bies","spawn_utopiec","spawn_gryffin", "spawn_witch" };
        actions = new string[] { "info", monsterIDs[ Random.Range(0, monsterIDs.Length) ] };
    }
}
