using UnityEngine;

public class ModifyBuildingsGods : MonoBehaviour
{
    private static ModifyBuildingsGods _instance;

    public static ModifyBuildingsGods Instance
    {
        get
        {
            if (_instance == null)
            {
                Debug.LogError("ModifyBuildingsGods is null");
            }
            return _instance;
        }
    }

    void Awake()
    {
        _instance = this;
    }

    [Header("Perun")]
    public int modifyBuildingsPerun;
    public float perunWoodModifier;
    public float perunFoodModifier;
    [Header("Swarog")]
    public int modifyBuildingsSwarog;
    public float swarogWoodModifier;
    public float swarogFoodModifier;
    [Header("Marzanna")]
    public int modifyBuildingsMarzanna;
    public float marzannaWoodModifier;
    public float marzannaFoodModifier;
    [Header("Dziewanna")]
    public int modifyBuildingsDziewanna;
    public float dziewannaWoodModifier;
    public float dziewannaFoodModifier;
    [Header("Swietowid")]
    public int modifyBuildingsSwietowid;
    public float swietowidWoodModifier;
    public float swietowidFoodModifier;
}
