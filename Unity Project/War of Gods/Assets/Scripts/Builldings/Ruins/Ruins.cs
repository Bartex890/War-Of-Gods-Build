using UnityEngine;

[CreateAssetMenu(fileName = "Ruins", menuName = "Buildings/Ruins")]
public class Ruins : Building
{
    public Ruins(string name, int teamID) : base(name, teamID, 1, "ruins")
    {
        GameManager.onTurnEnded += ProcessTurn;
        actions = new string[] { "info", "spawn_warrior" };
    }

    public override void Init()
    {

    }
}
