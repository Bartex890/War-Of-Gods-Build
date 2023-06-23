using UnityEngine;

[CreateAssetMenu(fileName = "NewBase", menuName = "Buildings/Base")]
public class Base : ProductiveBuilding
{
    public Base(string name, int teamID) : base(name, teamID, 1)
    {
        GameManager.onTurnEnded += ProcessTurn;
        actions = new string[] { "info", "spawn_warrior" };
    }

    /*public override void ProcessTurn()
    {
        base.ProcessTurn();
        if (GameManager.Instance.GetCurrentTeam() == teamID)
        {
            GameManager.Instance.GetCurrentTeamObj().AddFood(5);
            GameManager.Instance.GetCurrentTeamObj().AddWood(5);
            GameManager.Instance.GetCurrentTeamObj().AddGold(5);
        }
    }*/

    public override void Init()
    {
        base.Init();
        //GameManager.onTurnEnded += ProcessTurn;
    }
}
