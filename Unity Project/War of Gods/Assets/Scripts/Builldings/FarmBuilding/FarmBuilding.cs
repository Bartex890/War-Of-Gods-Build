
using UnityEngine;

[CreateAssetMenu(fileName = "newFarm", menuName = "Buildings/Farm")]
public class FarmBuilding : Building
{
    [SerializeField]
    private int _farmPower;

    public FarmBuilding(string name, int teamID, int farmPower) : base(name, teamID, 2, "farm")
    {
        _farmPower = farmPower;

        GameManager.onTurnEnded += ProcessTurn;
    }

    public override void ProcessTurn()
    {
        base.ProcessTurn();
        if (GameManager.Instance.GetCurrentTeam() == teamID)
        {
            GameManager.Instance.GetCurrentTeamObj().AddFood(_farmPower);
        }
        
    }

    public override void Init()
    {
        base.Init();
        GameManager.onTurnEnded += ProcessTurn;
    }
}
