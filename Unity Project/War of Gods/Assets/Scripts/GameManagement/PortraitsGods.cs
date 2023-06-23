using UnityEngine;

public class PortraitsGods : MonoBehaviour
{
    private static PortraitsGods _instance;

    public static PortraitsGods Instance
    {
        get
        {
            if (_instance == null)
            {
                Debug.LogError("PortraitsGods is null");
            }
            return _instance;
        }
    }

    void Awake()
    {
        _instance = this;
    }

    public Sprite perunPortrait;
    public Sprite swarogPortrait;
    public Sprite marzannaPortrait;
    public Sprite dziewannaPortrait;
    public Sprite swietowidPortrait;
}
