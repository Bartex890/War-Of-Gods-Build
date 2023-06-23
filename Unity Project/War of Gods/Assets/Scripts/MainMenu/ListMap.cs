using UnityEngine;
using System.Collections.Generic;

public class ListMap : MonoBehaviour
{
    [System.Serializable]
    private struct DictionaryTypeMap
    {
        public string name;
        public TypeMap typeMap;
    }
    private static ListMap _instance;
    
    [System.Serializable]
    public struct TypeMap
    {

        public int numberOfTeams;
    }
    [SerializeField]
    private DictionaryTypeMap[] _dictionaryTypeMap;
    public Dictionary<string,TypeMap> typeMap = new Dictionary<string,TypeMap>();
    public static ListMap Instance
    {
        get
        {
            if (_instance == null)
            {
                Debug.LogError("ListMap is null");
            }
            return _instance;
        }
    }

    private void Awake()
    {
        _instance = this;
        foreach (DictionaryTypeMap dd in _dictionaryTypeMap)
            typeMap.Add(dd.name, dd.typeMap);
    }
}
