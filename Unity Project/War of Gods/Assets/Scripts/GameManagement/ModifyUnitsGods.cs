using UnityEngine;

public class ModifyUnitsGods : MonoBehaviour
{
    private static ModifyUnitsGods _instance;

    public static ModifyUnitsGods Instance
    {
        get
        {
            if (_instance == null)
            {
                Debug.LogError("ModifyUnitsGods is null");
            }
            return _instance;
        }
    }

    void Awake()
    {
        _instance = this;
    }
    [Header("Perun Attack")]
    public int modifyWarriorPerun;
    public int modifyArcherPerun;
    public int modifyWorshipperPerun;
    [Header("Swaróg Attack")]
    public int modifyWarriorSwarog;
    public int modifyArcherSwarog;
    public int modifyWorshipperSwarog;
    [Header("Marzanna Attack")]
    public int modifyWarriorMarzanna;
    public int modifyArcherMarzanna;
    public int modifyWorshipperMarzanna;
    [Header("Dziewanna Attack")]
    public int modifyWarriorDziewanna;
    public int modifyArcherDziewanna;
    public int modifyWorshipperDziewanna;
    [Header("Swiêtowid Attack")]
    public int modifyWarriorSwietowid;
    public int modifyArcherSwietowid;
    public int modifyWorshipperSwietowid;

    [Header("Perun HP")]
    public int modifyHPWarriorPerun;
    public int modifyHPArcherPerun;
    public int modifyHPWorshipperPerun;
    [Header("Swaróg HP")]
    public int modifyHPWarriorSwarog;
    public int modifyHPArcherSwarog;
    public int modifyHPWorshipperSwarog;
    [Header("Marzanna HP")]
    public int modifyHPWarriorMarzanna;
    public int modifyHPArcherMarzanna;
    public int modifyHPWorshipperMarzanna;
    [Header("Dziewanna HP")]
    public int modifyHPWarriorDziewanna;
    public int modifyHPArcherDziewanna;
    public int modifyHPWorshipperDziewanna;
    [Header("Swiêtowid HP")]
    public int modifyHPWarriorSwietowid;
    public int modifyHPArcherSwietowid;
    public int modifyHPWorshipperSwietowid;
}
