using System.Collections.Generic;
using UnityEngine;

public class UnitDataHolder : MonoBehaviour
{
    [System.Serializable]
    private struct DictionaryData{
        public string name;
        public UnitData unitData;
    }

    [System.Serializable]
    public struct UnitData
    {
        public Sprite sprite;
        public RuntimeAnimatorController anim;
        [TextArea(5,10)]
        public string description;
        public Projectile projectile;
        public Unit unit;
    }

    [SerializeField]
    private DictionaryData[] _dictionaryData;

    public Dictionary<string, UnitData> unitData = new Dictionary<string, UnitData>();

    private static UnitDataHolder _instance;

    public static UnitDataHolder Instance
    {
        get
        {
            if (_instance == null)
            {
                Debug.LogError("UnitDataHolder is null");
            }
            return _instance;
        }
    }

    private void Awake()
    {
        _instance = this;

        foreach(DictionaryData dd in _dictionaryData)
            unitData.Add(dd.name, dd.unitData);
    }
}
