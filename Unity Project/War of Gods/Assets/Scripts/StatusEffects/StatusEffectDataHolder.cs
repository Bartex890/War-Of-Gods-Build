using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusEffectDataHolder : MonoBehaviour
{
    [System.Serializable]
    private struct DictionaryData {
        public string name;
        [SerializeReference]
        public StatusEffect statusEffectData;
    }

    [SerializeField]
    private DictionaryData[] _dictionaryData;

    public Dictionary<string, StatusEffect> statusEffectData = new Dictionary<string, StatusEffect>();

    private static StatusEffectDataHolder _instance;

    public static StatusEffectDataHolder Instance
    {
        get
        {
            if (_instance == null)
            {
                Debug.LogError("StatusEffectDataHolder is null");
            }
            return _instance;
        }
    }

    private void Awake()
    {
        _instance = this;

        foreach(DictionaryData dd in _dictionaryData)
            statusEffectData.Add(dd.name, dd.statusEffectData);
    }
}
