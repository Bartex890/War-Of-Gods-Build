using UnityEngine;
using UnityEngine.UI;

public class UnitsSprites : MonoBehaviour
{
    private static UnitsSprites _instance;

    public static UnitsSprites Instance
    {
        get
        {
            if (_instance == null)
            {
                Debug.LogError("UnitsSprites is null");
            }
            return _instance;
        }
    }

    void Awake()
    {
        _instance = this;
    }

    [Header("Basic Units")]
    public Sprite worshipperSprite;
    public Sprite archerSprite;
    public Sprite warriorSprite;
    public Sprite cavarlySprite;
    [Header("Monsters")]
    public Sprite basilicsSprite;
    public Sprite bearSprite;
    public Sprite biesSprite;
    public Sprite gryffinSprite;
    public Sprite LadyMiddaySprite;
    public Sprite LeshySprite;
    public Sprite RusalkaSprite;
    public Sprite utopiecSprite;
    public Sprite werewolfSprite;
    public Sprite witchSprite;
    public Sprite wolfSprite;
    [Header("Gods")]
    public Sprite dziewannaSprite;
    public Sprite marzannaSprite;
    public Sprite perunSprite;
    public Sprite swarogSprite;
    public Sprite swietowidSprite;

}
