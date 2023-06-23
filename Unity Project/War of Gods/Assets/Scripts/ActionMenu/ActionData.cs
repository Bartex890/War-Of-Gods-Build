using UnityEngine;

[CreateAssetMenu(fileName = "newAction", menuName = "Actions/Action")]
public class ActionData : ScriptableObject
{
    new public string name;
    [TextArea(5, 10)]
    public string description;
    public Sprite icon;
    public string actionCode;
}
