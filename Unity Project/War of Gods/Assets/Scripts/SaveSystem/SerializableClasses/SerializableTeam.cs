using static Gods;

[System.Serializable]
public class SerializableTeam
{
    public float lastCameraX, lastCameraY;
    public Factions god;
    public int amountWood;
    public int amountGold;
    public int amountFood;
    public int amountFaithPoints;
    public bool[,] visitedTiles;
    public bool isDefeat;
}
