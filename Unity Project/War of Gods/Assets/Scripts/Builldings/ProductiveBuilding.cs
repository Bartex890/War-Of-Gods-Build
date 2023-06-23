using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newProductiveBuilding", menuName = "Buildings/ProductiveBuilding")]
public class ProductiveBuilding : Building
{
    [SerializeField]
    private int _woodProduction;
    [SerializeField]
    private int _goldProduction;
    [SerializeField]
    private int _foodProduction;
    [SerializeField]
    private int _faithProduction;

    public ProductiveBuilding(string name, int teamID, int buildingID = 2) : base(name, teamID, buildingID, "farm")
    {

        GameManager.onTurnEnded += ProcessTurn;
    }

    public override void ProcessTurn()
    {
        base.ProcessTurn();
        if (GameManager.Instance.GetCurrentTeam() == teamID)
        {   //tutaj doda³em modyfikatory z eventow
            GameManager.Instance.GetCurrentTeamObj().AddWood((int)Mathf.Ceil(_woodProduction * GameManager.Instance.GetTeam(teamID).GetFaction().GetGodWoodModifier()*EventModifier.Instance.percentBuffWood));
            GameManager.Instance.GetCurrentTeamObj().AddGold((int)Mathf.Ceil(_goldProduction * EventModifier.Instance.percentBuffGold));
            GameManager.Instance.GetCurrentTeamObj().AddFood((int)Mathf.Ceil(_foodProduction * GameManager.Instance.GetTeam(teamID).GetFaction().GetGodFoodModifier() * EventModifier.Instance.percentBuffFood));
            GameManager.Instance.GetCurrentTeamObj().AddFaithPoints((int)Mathf.Ceil(_faithProduction * EventModifier.Instance.percentBuffFaithPoints));
        }
    }

    public override void Init()
    {
        base.Init();
        GameManager.onTurnEnded += ProcessTurn;
    }

    public int GetProducedWood()
    {
        return (int)Mathf.Ceil(_woodProduction * GameManager.Instance.GetTeam(teamID).GetFaction().GetGodWoodModifier());
    }
    public int GetProducedGold()
    {
        return _goldProduction;
    }
    public int GetProducedFood()
    {
        return (int)Mathf.Ceil(_foodProduction * GameManager.Instance.GetTeam(teamID).GetFaction().GetGodFoodModifier());
    }
    public int GetProducedFaith()
    {
        return _faithProduction;
    }
}
