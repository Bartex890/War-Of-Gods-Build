using UnityEngine;

public class EventModifier : MonoBehaviour
{
    public static EventModifier _instance;
    public static EventModifier Instance
    {
        get
        {
            if (_instance == null)
            {
                Debug.LogError("EventModifier is null");
            }
            return _instance;
        }
    }


    private void Awake()
    {
        _instance = this;

    }

    [Header("Building")]
    public float percentBuffWood=1f;
    public float percentBuffGold=1f;
    public float percentBuffFood=1f;
    public float percentBuffFaithPoints=1f;

    [Header("Units")]
    public float percentBuffRangeMove = 1f;
}
