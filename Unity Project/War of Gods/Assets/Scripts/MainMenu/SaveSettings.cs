using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveSettings : MonoBehaviour
{
    private string _nameGame;
    private string _map;
    private int _numberOfTeams;
    private List<Gods> _factions;
    private MapManager _mapManager;
    private GameManager _gameManager;

    private SavedGame _saveToLoad;
    private string _saveName;

    private static SaveSettings _instance;
    public static SaveSettings Instance
    {
        get
        {
            if (_instance == null)
            {
                //Debug.LogError("SaveSettings is null");
            }
            return _instance;
        }
    }
    private void Awake()
    {
        _instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void SetObjects(MapManager mapManager, GameManager gameManager)
    {
        _mapManager = mapManager;
        _gameManager = gameManager;
    }
    public void Save(string nameGame,string map,int numberOfTeams, List<Gods> faction)
    {
        this._map = map;
        _numberOfTeams = numberOfTeams;
        _factions = faction;
        _nameGame = nameGame;
    }


    public void OnLevelWasLoaded(int level)
    {
        if (level > 1)
        {
            if (_saveToLoad != null)
            {
                _gameManager.SetTeams(_saveToLoad);
                _mapManager.SetMap(_saveToLoad);
            }
            else
            {
                _gameManager.SetTeams(_numberOfTeams, _factions, _nameGame);
                _mapManager.SetMap(_map);
            }
            
        } else
        {
            _saveToLoad = null;
        }
    }

    public string GetGameName()
    {
        return _nameGame;
    }

    public string GetMapName()
    {
        return _map;
    }

    public int GetTeamAmount()
    {
        return _numberOfTeams;
    }

    public SavedGame GetGameToLoad()
    {
        return _saveToLoad;
    }

    public void SetSaveFile(string saveName)
    {
        _saveName = saveName;
    }

    public void LoadSaveFile()
    {
        _saveToLoad = SaveManager.Load(_saveName);

        List<Gods> factions = new List<Gods>();

        foreach (SerializableTeam team in _saveToLoad.teams)
        {
            factions.Add(new Gods() { gods = team.god });
        }

        Save(_saveToLoad.gameName,
            _saveToLoad.mapName,
            _saveToLoad.teams.Length,
            factions
            );
        SceneManager.LoadScene("MapSelectionTest", LoadSceneMode.Single);
    }
}
