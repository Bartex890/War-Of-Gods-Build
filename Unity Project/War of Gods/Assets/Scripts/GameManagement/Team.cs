using UnityEngine;
using System;

[System.Serializable]
public class Team
{
    private CameraController cameraController;
    private Vector3 lastPositionCamera;
    private Gods _faction; //it is enum
    private int _amountWood;
    private int _amountGold;
    private int _amountFood;
    private int _amountFaithPoints;
    public bool[,] visitedTiles;
    public bool isDefeat;
    public bool isGodRespawned;
    //gods = (Factions) Enum.ToObject(typeof(Factions), input);

    public Team(Gods god)
    {
        cameraController = GameObject.FindObjectOfType<CameraController>();
        //SetLastPositionCamera();
        _amountWood = 0;
        _amountGold = 0;
        _amountFood = 0;
        _amountFaithPoints = 0;
        isDefeat = false;
        isGodRespawned = false;
        _faction = new Gods { gods=god.gods };
    }
    public Team(SerializableTeam sTeam)
    {
        cameraController = GameObject.FindObjectOfType<CameraController>();
        _amountWood = sTeam.amountWood;
        _amountGold = sTeam.amountGold;
        _amountFood = sTeam.amountFood;
        _amountFaithPoints = sTeam.amountFaithPoints;
        isDefeat = sTeam.isDefeat;
        _faction = new Gods { gods = sTeam.god };

        lastPositionCamera = new Vector3(sTeam.lastCameraX, sTeam.lastCameraY, -10f);
        visitedTiles = sTeam.visitedTiles;
    }
    
    public void SetLastPositionCamera()
    {
        this.lastPositionCamera = cameraController.GetTransformPositionCamera().position;
    }
    public void SetCameraPosition()
    {
        cameraController.SetTransformPosition(lastPositionCamera);
    }
    public void SetCameraPosition(Vector3 pos)
    {
        cameraController.SetTransformPosition(pos);
        this.lastPositionCamera = pos;
    }

    public Vector3 GetLastCameraPosition()
    {
        return lastPositionCamera;
    }

    public int GetAmountWood()
    {
        return _amountWood;
    }
    public int GetAmountGold()
    {
        return _amountGold;
    }
    public int GetAmountFood()
    {
        return _amountFood;
    }
    public int GetAmountFaithPoints()
    {
        return _amountFaithPoints;
    }
    public void AddWood(int amount)
    {
        _amountWood += amount;
        GameManager.Instance.PlusSetWoodText(amount);
    }
    public void AddGold(int amount)
    {
        _amountGold += amount;
        GameManager.Instance.PlusSetGoldText(amount);
    }
    public void AddFood(int amount)
    {
        _amountFood += amount;
        GameManager.Instance.PlusSetFoodText(amount);
    }
    public void AddFaithPoints(int amount)
    {
        _amountFaithPoints += amount;
    }

    public Gods GetFaction()
    {
        return _faction;
    }

    
}
