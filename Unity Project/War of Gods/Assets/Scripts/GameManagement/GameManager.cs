using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Linq;
using UnityEngine.Rendering.PostProcessing;

public class GameManager : MonoBehaviour
{
    [Header("UI Turn and Team")]
    [SerializeField]
    private TextMeshProUGUI currentTeamText;
    [SerializeField]
    private TextMeshProUGUI currentTeamTextUI;
    [SerializeField]
    private TextMeshProUGUI currentTurnTextUI;
    [SerializeField]
    private GameObject panelEndTurn;
    [SerializeField]
    private Image currentGodPortrait;
    [SerializeField]
    private TextMeshProUGUI currentTurnText;
    [SerializeField]
    private TextMeshProUGUI nameGameUI;
    [SerializeField]
    private Image timeOfDay;
    [SerializeField]
    private List<Sprite> images = new List<Sprite>();
    [SerializeField]
    private GameObject _defeatWindow;
    [SerializeField]
    private GameObject _winWindow;

    [SerializeField]
    private PostProcessVolume _timeOfDayPostProcessVolume;
    [SerializeField]
    private PostProcessProfile[] _timeOfDayPostProcessProfiles;

    [HideInInspector]
    public int currentTeam;
    private int numberOfTeams;
    private List<Team> teams;
    private int countTeams;
    private int subTurnID;
    private int currentTurn;
    public int currentDay;

    [Header("UI Materials")]
    [SerializeField]
    private TextMeshProUGUI currentWood;
    [SerializeField]
    private TextMeshProUGUI currentGold;
    [SerializeField]
    private TextMeshProUGUI currentFood;
    [SerializeField]
    private TextMeshProUGUI plusCurrentWood;
    private int plusCurrentWoodCounter;
    [SerializeField]
    private TextMeshProUGUI plusCurrentGold;
    private int plusCurrentGoldCounter;
    [SerializeField]
    private TextMeshProUGUI plusCurrentFood;
    private int plusCurrentFoodCounter;
    [SerializeField]
    private TextMeshProUGUI currentFaithPoints;

    private int _numberOfDefeats;

    //map data
    private MapManager _mapManager;

    //singleton stuff
    private static GameManager _instance;

    //camera
    private CameraController _cameraController;

    private SaveSettings _saveSettings;
    private string _nameGame;

    [SerializeField]
    private Camera _minimapCamera;

    public static GameManager Instance
    {
        get
        {
            if (_instance == null)
            {
                Debug.LogError("GameManager is null");
            }
            return _instance;
        }
    }
    private void Awake()
    {
        _instance = this;

        onTurnEnded = null;
        onRoundEnded = null;

        if (onTurnEnded != null)
        {
            Debug.LogWarning("1 Subscribers: " + string.Join(", ", onTurnEnded?.GetInvocationList()?.Select(d => d.Target.ToString()).ToArray()));
            //Debug.LogWarning("on turn ended: " + onTurnEnded.GetInvocationList());
        }

        _mapManager = FindObjectOfType<MapManager>();
        
        onTurnEnded += () => LocalSaveUtility.Instance.SaveTheGame();
        _cameraController = FindObjectOfType<CameraController>();
        _saveSettings = SaveSettings.Instance;
        if (_saveSettings != null)
        {
            _saveSettings.SetObjects(_mapManager, this);
        }
        if(_saveSettings.GetMapName()=="Island II")
        {
            _minimapCamera.orthographicSize /= 2;
            _minimapCamera.transform.position = new Vector3(_minimapCamera.transform.position.x-12.5f, _minimapCamera.transform.position.y+12.5f,-10);
        }
        SetCurrentTeam(0);
        SetCurrentTeamText();
        //numberOfTeams = 2; //It's only for testing, so this value will be taken from method
        //Dictionary<Vector2Int, Building>.ValueCollection tempValues = _mapManager.map.getAllBuildings().Values;
        //Dictionary<Vector2Int, Building> temp = _mapManager.map.getAllBuildings();
        //for (int i = 0; i < numberOfTeams; i++)
        //{
        //    Team team = new Team(i);
        //    teams.Add(team);
        //}
        currentDay = 0;
    }   
    public void SetTeams(int amount, List<Gods> faction, string nameGame)
    {
        _nameGame = nameGame;
        nameGameUI.text = _nameGame;
        teams = new List<Team>();
        numberOfTeams = amount;
        for(int i =0; i < numberOfTeams; i++)
        {
            teams.Add(new Team(faction[i]));
        }
    }
    public void SetTeams(SavedGame save)
    {
        _nameGame = save.gameName;
        nameGameUI.text = _nameGame;
        teams = new List<Team>();
        numberOfTeams = save.teams.Length;
        for (int i = 0; i < numberOfTeams; i++)
        {
            teams.Add(new Team(save.teams[i]));
        }
    }

    private void Start()
    {
        if (onTurnEnded != null)
        {
            Debug.LogWarning("Subscribers: " + string.Join(", ", onTurnEnded?.GetInvocationList()?.Select(d => d.Target.ToString()).ToArray()));
            //Debug.LogWarning("on turn ended: " + onTurnEnded.GetInvocationList());
        }
            

        MapManager.Instance.LoadMap();

        if (SaveSettings.Instance.GetGameToLoad() != null)
        {
            SavedGame save = SaveSettings.Instance.GetGameToLoad();
            subTurnID = save.currentTeam + 1;
            currentTurn = save.currentRound;

            SetCurrentTeam(save.currentTeam);
            SetCurrentTeamText();
            currentDay = (save.currentRound - 1) % 6;
            currentTeam = save.currentTeam;

            teams[currentTeam].SetCameraPosition();

            SetCurrentTeamText();
            SetCurrentTurnText();

            MapLoader.LoadStatusEffectsFromSave(save);
        }
        else
        {
            subTurnID = 1;
            currentTurn = 1;
            _numberOfDefeats = 0;
            int t = 0;
            foreach (KeyValuePair<Vector2Int, Building> kvp in _mapManager.map.getAllBuildings())
            {
                if (kvp.Value.buildingType == 1)
                {
                    Vector3 tempv = new Vector3(kvp.Key.x, -kvp.Key.y, 0);
                    Debug.Log(tempv);
                    teams[t].SetCameraPosition(tempv);
                    /*if(t>teams.Count)*/
                    t++;
                }
            }

            if (teams == null) SetTeams(2, new List<Gods>()
            {
                new Gods() { gods = Gods.Factions.dziewanna },
                new Gods() { gods = Gods.Factions.swietowid }
            }, "test");

            teams[0].SetCameraPosition();

            foreach (Team team in teams)
            {
                if (team.visitedTiles == null)
                {
                    team.visitedTiles = new bool[_mapManager.map.tiles.GetLength(0), _mapManager.map.tiles.GetLength(1)];
                }

            }
        }

        countTeams = teams.Count;
        SetPortraitGods();
        onRoundEnded += () => Events.Instance.RandomEvent();
        onRoundEnded += () => UnitsTimeOfDay();
        onRoundEnded += () => _mapManager.SpawnMonsters(2);
        onTurnEnded += () => Events.Instance.SetActiveWindowEvents();
        onTurnEnded += () => _mapManager.map.ResetMovementOptions();
        onTurnEnded += () => _mapManager.map.IsOcupationUnit();
        onTurnEnded += () => _mapManager.map.ResetIsTarget();
        onTurnEnded += () => _mapManager.ResetDrawEndTurn();
        onTurnEnded += () => ActionMenuManager.Instance.HideMenu();
        onTurnEnded();
        RefreshResourceDisplays();
        panelEndTurn.SetActive(true);
        Events.Instance.RandomEvent();

        _timeOfDayPostProcessVolume.profile = _timeOfDayPostProcessProfiles[currentDay];

        _mapManager.RefreshAllIndicatorColors();
        _mapManager.RefreshAllHealthbars();
    }
    private void Update()
    {
        if (panelEndTurn.activeSelf ==true)
        {
            Time.timeScale=0;
        }
        else
        {
            Time.timeScale=1;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            foreach (Vector2Int location in _mapManager.map.getAllBuildings().Keys)
            {
                if (_mapManager.map.getBuilding(location).teamID == currentTeam)
                {
                        Vector3 tempv = new Vector3(location.x, -location.y, 0);
                        _cameraController.SetTransformPosition(tempv);
                        break;
                    
                }
            }
        }
    }
    // Team
    public int GetCurrentTeam()
    {
        return currentTeam;
    }

    public Team GetCurrentTeamObj()
    {
        return teams[currentTeam];
    }

    public Team GetTeam(int id)
    {
        return teams[id];
    }

    private void SetCurrentTeam(int currentTeam)
    {
        this.currentTeam=currentTeam;
    }
    private void SetCurrentTeamText()
    {
        currentTeamText.text = "Now play Team: "+(GetCurrentTeam()+1);
        currentTeamTextUI.text = "Team "+(GetCurrentTeam()+1);
        
    }
    private void SetPortraitGods()
    {
        if (teams != null)
            currentGodPortrait.sprite = teams[currentTeam].GetFaction().GetPortait();
    }
    // Turn
    private int GetCurrentTurn()
    {
        return currentTurn;
    }
    private void SetCurrentTurnText()
    {
        currentTurnText.text = "Turn " + GetCurrentTurn();
        currentTurnTextUI.text = "Turn " + GetCurrentTurn();
    }
    //materials
    private int GetWoodTeam()
    {
        return teams[GetCurrentTeam()].GetAmountWood();
    }
    private int GetGoldTeam()
    {
        return teams[GetCurrentTeam()].GetAmountGold();
    }
    private int GetFoodTeam()
    {
        return teams[GetCurrentTeam()].GetAmountFood();
    }
    private int GetFaithPointsTeam()
    {
        return teams[GetCurrentTeam()].GetAmountFaithPoints();
    }
    private void SetWoodText(int amount)
    {
        currentWood.text =(GetWoodTeam()+amount).ToString();
    }
    private void SetGoldText(int amount)
    {
        currentGold.text = (GetGoldTeam() + amount).ToString();
    }
    private void SetFoodText(int amount)
    {
        currentFood.text = (GetFoodTeam() + amount).ToString();
    }
    public void PlusSetWoodText(int amount)
    {
        if (amount <= 0) return;
        plusCurrentWoodCounter += amount;
        plusCurrentWood.text = "+" + (plusCurrentWoodCounter.ToString());
    }
    public void PlusSetGoldText(int amount)
    {
        if (amount <= 0) return;
        plusCurrentGoldCounter += amount;
        plusCurrentGold.text = "+" + (plusCurrentGoldCounter.ToString());
    }
    public void PlusSetFoodText(int amount)
    {
        if (amount <= 0) return;
        plusCurrentFoodCounter += amount;
        plusCurrentFood.text = "+" + (plusCurrentFoodCounter.ToString());
    }
    private void SetFaithPointsText(int amount)
    {
        currentFaithPoints.text = (GetFaithPointsTeam() + amount).ToString();
    }
    private void RefreshPlusSetResource()
    {
        plusCurrentWoodCounter = 0;
        plusCurrentGoldCounter = 0;
        plusCurrentFoodCounter = 0;
        Debug.Log(new System.Diagnostics.StackTrace().GetFrame(0).GetMethod().Name.ToString());
    }
    public int GetCurrentRound()
    {
        return currentTurn;
    }

    public void RefreshResourceDisplays()
    {
        SetWoodText(0);
        SetGoldText(0);
        SetFoodText(0);
        SetFaithPointsText(0);
    }

    public void UnitsTimeOfDay()
    {
        Dictionary<Vector2Int, Unit> dictUnits = _mapManager.map.getAllUnits();
        List<Unit> units = dictUnits.Values.ToList();
        foreach (Unit unit in units)
        {
            switch (currentDay % 6)
            {
                case 0:
                    if (unit.GetUnitType() == Unit.UnitType.werewolf)
                    {
                        //unit.SetHP(unit.getBaseHP());
                        unit.SetBaseHPmultiplier(1f);
                        if (unit.getHP() < unit.getBaseHP())
                        {
                            unit.numberOfUnits--;
                        }
                        Dictionary<Vector2Int, Unit> temp = _mapManager.map.getAllUnits();
                        Vector2Int pos = temp.FirstOrDefault(x => x.Value == unit).Key;
                        if (unit.numberOfUnits < 1)
                        {

                            _mapManager.DeleteUnitGraphic(pos);
                            _mapManager.map.RemoveUnit(pos);

                        }
                        else
                        {

                            unit.SetHP(unit.getBaseHP());
                            _mapManager.RefreshHealthbar(pos);
                            _mapManager.TransformationWerewolf(pos, unit, false);
                        }
                        int baseAttack = unit.getBaseStrengthAttack();
                        unit.SetAttackStrength(baseAttack);
                        unit.SetMoveRange(unit.getBaseMoveRange());
                    }
                    if (unit.GetUnitType() == Unit.UnitType.ladyMidday) 
                    {
                        int baseAttack = unit.getBaseStrengthAttack();
                        unit.SetAttackStrength((int)Mathf.Ceil(baseAttack + baseAttack * 0.5f));
                    } break;
                case 1:
                    if (unit.GetUnitType() == Unit.UnitType.werewolf)
                    {
                        unit.SetBaseHPmultiplier(1f);
                        int baseAttack = unit.getBaseStrengthAttack();
                        unit.SetAttackStrength(baseAttack);
                        unit.SetMoveRange(unit.getBaseMoveRange());
                    }
                    if (unit.GetUnitType() == Unit.UnitType.ladyMidday)
                    {
                        int baseAttack = unit.getBaseStrengthAttack();
                        unit.SetAttackStrength(baseAttack*2);
                    }
                    break;
                case 2:
                    if (unit.GetUnitType() == Unit.UnitType.werewolf)
                    {
                        unit.SetBaseHPmultiplier(1f);
                        int baseAttack = unit.getBaseStrengthAttack();
                        unit.SetAttackStrength(baseAttack);
                        unit.SetMoveRange(unit.getBaseMoveRange());
                    }
                    if (unit.GetUnitType() == Unit.UnitType.ladyMidday)
                    {
                        int baseAttack = unit.getBaseStrengthAttack();
                        unit.SetAttackStrength((int)Mathf.Ceil(baseAttack + baseAttack * 0.5f));
                    }
                    break;
                case 3: 
                    if (unit.GetUnitType() == Unit.UnitType.werewolf) 
                    {
                        unit.SetBaseHPmultiplier(2f);
                        unit.SetHP(unit.getBaseHP());
                        int baseAttack = unit.getBaseStrengthAttack();
                        unit.SetAttackStrength(baseAttack * 2);
                        unit.SetMoveRange(unit.getBaseMoveRange() * 2);
                        Dictionary<Vector2Int, Unit> temp = _mapManager.map.getAllUnits();
                        Vector2Int pos = temp.FirstOrDefault(x => x.Value == unit).Key;
                        _mapManager.RefreshHealthbar(pos);
                        _mapManager.TransformationWerewolf(pos, unit, true);
                    }
                    if (unit.GetUnitType() == Unit.UnitType.ladyMidday)
                    {
                        int baseAttack = unit.getBaseStrengthAttack();
                        unit.SetAttackStrength(baseAttack);
                    }
                    break;
                case 4:
                    if (unit.GetUnitType() == Unit.UnitType.werewolf)
                    {
                        unit.SetBaseHPmultiplier(2f);
                        int baseAttack = unit.getBaseStrengthAttack();
                        unit.SetAttackStrength(baseAttack * 2);
                        unit.SetMoveRange(unit.getBaseMoveRange() * 2);
                    }
                    if (unit.GetUnitType() == Unit.UnitType.ladyMidday)
                    {
                        int baseAttack = unit.getBaseStrengthAttack();
                        unit.SetAttackStrength((int)Mathf.Ceil(baseAttack - baseAttack * 0.5f));
                    }
                    break;
                case 5:
                    if (unit.GetUnitType() == Unit.UnitType.werewolf)
                    {
                        unit.SetBaseHPmultiplier(2f);
                        int baseAttack = unit.getBaseStrengthAttack();
                        unit.SetAttackStrength(baseAttack * 2);
                        unit.SetMoveRange(unit.getBaseMoveRange() * 2);
                    }
                    if (unit.GetUnitType() == Unit.UnitType.ladyMidday)
                    {
                        int baseAttack = unit.getBaseStrengthAttack();
                        unit.SetAttackStrength(baseAttack);
                    }
                    break;

            }
        }
    }


    public static Action onTurnEnded;
    public static Action onRoundEnded;
    public void EndTurn()
    {
        RefreshPlusSetResource();
        teams[GetCurrentTeam()].SetLastPositionCamera();
        nextTeam:
        SetCurrentTeam(currentTeam + 1);
        if (GetCurrentTeam() >= numberOfTeams)
        {
            currentDay++;
            if (currentDay % 6 == 0)
            {
                currentDay = 0;


            }
            SetCurrentTeam(0);
            onRoundEnded();
        }
        if ((_numberOfDefeats + 1) >= numberOfTeams && !teams[GetCurrentTeam()].isDefeat)
        {
            _winWindow.SetActive(true);
        }
        if (teams[GetCurrentTeam()].isDefeat)
        {
            goto nextTeam;
        }
        if (IsDefeat())
        {
            if (!teams[GetCurrentTeam()].isDefeat)
            {
                teams[GetCurrentTeam()].isDefeat = true;
                _numberOfDefeats++;
                _defeatWindow.SetActive(true);
            }
            goto nextTeam;
        }
        

        timeOfDay.sprite = images[currentDay];
        _timeOfDayPostProcessVolume.profile = _timeOfDayPostProcessProfiles[currentDay];

        teams[GetCurrentTeam()].SetCameraPosition();
        SetCurrentTeamText();
        SetPortraitGods();
        if (subTurnID == countTeams)
        {
            currentTurn++;
            SetCurrentTurnText();
            subTurnID = 0;
        }
        subTurnID++;
        panelEndTurn.SetActive(true);
        
        onTurnEnded();

        RefreshResourceDisplays();

        //Setting The team displays to their respective colours
        _mapManager.RefreshAllIndicatorColors();
    }

    private bool IsDefeat()
    {
        for (int i = _mapManager.map.getAllBuildings().Count - 1; i >= 0; i--)
        {
            var building = _mapManager.map.getAllBuildings().ToArray()[i];
            if (building.Value.teamID == GetCurrentTeam())
            {
                return false;
            }
        }
        for (int i = _mapManager.map.getAllUnits().Count - 1; i >= 0; i--)
        {
            var unit = _mapManager.map.getAllUnits().ToArray()[i];
            if (unit.Value.teamID == GetCurrentTeam())
            {
                return false;
            }
        }
        return true;
    }

    public List<Team> GetAllTeams()
    {
        return teams;
    }
}
