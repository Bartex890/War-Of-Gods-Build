using System.Collections.Generic;
using UnityEngine;

public class BuildingDataHolder : MonoBehaviour
{
    [System.Serializable]
    private struct DictionaryData
    {
        public string name;
        public BuildingData buildingData;
    }

    [System.Serializable]
    public struct BuildingData{
        public Sprite[] sprite;
        [TextArea(5, 10)]
        public string description;
        public Building building;
    }

    [SerializeField]
    private DictionaryData[] _buildingData;

    public Dictionary<string, BuildingData> buildingData = new Dictionary<string, BuildingData>();

    private static BuildingDataHolder _instance;

    public static BuildingDataHolder Instance
    {
        get
        {
            if (_instance == null)
            {
                Debug.LogError("BuildingDataHolder is null");
            }
            return _instance;
        }
    }

    private void Awake()
    {
        _instance = this;

        foreach(DictionaryData dd in _buildingData)
            buildingData.Add(dd.name, dd.buildingData);
    }

    public static BuildingData GetBuildingData(string id)
    {
        return Instance.buildingData.GetValueOrDefault(id);
    }
}
