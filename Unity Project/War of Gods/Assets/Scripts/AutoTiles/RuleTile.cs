using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newRuleTile", menuName = "RuleTile")]
public class RuleTile : ScriptableObject
{
    [System.Serializable]
    public struct Rule
    {
        public Sprite[] sprite;
        [TextArea(minLines:3, maxLines:3)]
        public string rules;
    }

    public Sprite[] defaultSprite;
    public Rule[] rules;
}
