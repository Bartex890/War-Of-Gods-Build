using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionDataHolder : MonoBehaviour
{
    [System.Serializable]
    private struct DictionaryData {
        public string name;
        [SerializeReference]
        public ActionData actionData;
    }

    [SerializeField]
    private DictionaryData[] _dictionaryData;

    public Dictionary<string, ActionData> actionData = new Dictionary<string, ActionData>();

    private static ActionDataHolder _instance;

    public static ActionDataHolder Instance
    {
        get
        {
            if (_instance == null)
            {
                Debug.LogError("ActionDataHolder is null");
            }
            return _instance;
        }
    }

    private void Awake()
    {
        _instance = this;

        foreach(DictionaryData dd in _dictionaryData)
            actionData.Add(dd.name, dd.actionData);
    }
}
