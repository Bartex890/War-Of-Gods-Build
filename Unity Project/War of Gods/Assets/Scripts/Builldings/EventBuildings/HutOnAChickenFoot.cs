using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewHutOnAChickenFoot", menuName = "Buildings/HutOnAChickenFoot")]
public class HutOnAChickenFoot : Building
{
    public HutOnAChickenFoot(string name, int teamID) : base(name, teamID, 10, "hut_on_a_chicken_foot")
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

    }
}
