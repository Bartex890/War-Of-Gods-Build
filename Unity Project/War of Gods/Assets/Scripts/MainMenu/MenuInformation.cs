using UnityEngine;

[System.Serializable]
public class MenuInformation
{
    [TextArea(10,10)]
    public string informationSection;
    [TextArea(5, 10)]
    public string informationBuffs;
    public Sprite informationPortrait;
}
