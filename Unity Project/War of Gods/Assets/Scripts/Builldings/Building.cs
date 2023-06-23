using Unity.VisualScripting;
using UnityEngine;

public class Building : MapObject
{
    [HideInInspector]
    public int buildingType;
    private int _basePounding;  
    public int pounding;
    private string[] _hiddenActions;
    
    

    public Building(string name, int teamID, int buildingType, string buildingID, int pounding=100)
    {
        this.name = name;
        this.teamID = teamID;
        this.buildingType = buildingType;
        this.ID = buildingID;
        this._basePounding = pounding;
        this.pounding = this._basePounding;

        this.actions = new string[] { "info" };
    }
    public void DealDamageBuilding(int amount)
    {
        if (pounding <= 0)
        {
            return;
        }

        pounding -= amount;

        if (pounding <= 0)
        {
            SwapActionSets();
        }
    }
    public void SwitchTeam(int teamID)
    {
        this.teamID = teamID;
    }
    public virtual void ProcessTurn() {
        Debug.Log(new System.Diagnostics.StackTrace().GetFrame(0).GetMethod().Name.ToString());
    }

    public virtual void Init() {
        
        this._basePounding = GameManager.Instance.GetTeam(teamID).GetFaction().GetGodBuilding();
        
        _hiddenActions = new string[] { "fix" };
        this.pounding = this._basePounding;
    }

    public void SwapActionSets()
    {
        string[] temp = actions;
        actions = _hiddenActions;
        _hiddenActions = temp;
    }

    public void Fix()
    {
        SwapActionSets();
        
        this._basePounding = GameManager.Instance.GetTeam(teamID).GetFaction().GetGodBuilding();
        pounding = _basePounding;
        teamID = GameManager.Instance.GetCurrentTeam();
    }

    public void setBasePounding(int basePounding)
    {
        this._basePounding = basePounding;
    }

    public int GetBasePounding()
    {
        return _basePounding;
    }
}
