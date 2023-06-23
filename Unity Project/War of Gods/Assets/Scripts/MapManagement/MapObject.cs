using UnityEngine;

[System.Serializable]
public class MapObject : ScriptableObject
{
    new public string name;
    [HideInInspector]
    public int teamID;
    public string ID;
    public string[] actions = { "info" };
    public Vector2Int position;

    public virtual int GetVisionRange()
    {
        return 3;
    }
}
