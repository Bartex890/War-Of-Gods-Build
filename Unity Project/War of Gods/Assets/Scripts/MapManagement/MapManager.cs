using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
//using static UnityEditor.PlayerSettings;

public class MapManager : MonoBehaviour
{
    [Header("Map Data")]
    public Map map;
    
    [Header("Selection")]
    //selection
    [HideInInspector]
    public Vector2Int _selectedTile;
    private int _selectionType;
    private List<GameObject> _selectionMarkers;

    [SerializeField]
    private SpriteRenderer _baseRenderer;

    private Dictionary<Vector2Int, SpriteRenderer> _unitRenderers = new Dictionary<Vector2Int, SpriteRenderer>();
    private Dictionary<Vector2Int, SpriteRenderer> _buildingRenderers = new Dictionary<Vector2Int, SpriteRenderer>();

    [SerializeField]
    private SpriteRenderer _selectionMarker;
    [SerializeField]
    private SpriteRenderer _drawPositionUnit;

    [SerializeField]
    private Sprite _unitMarker;
    [SerializeField]
    private Sprite _tileMarker;
    [SerializeField]
    private Sprite _confirmationMarker;
    [SerializeField]
    private Sprite _positionOpt;
    [SerializeField]
    private Sprite _attackOpt;
    [SerializeField]
    private Sprite _splitOpt;
    //variables Movement and Attack
    private List<GameObject> drawLines = new List<GameObject>();
    private List<Vector2Int> listPositions = new List<Vector2Int>();
    private List<Vector2Int> listEnemyUnits = new List<Vector2Int>();
    private List<Vector2Int> listStackingUnits = new List<Vector2Int>();
    private Dictionary<Vector2Int,int> rangesDict = new Dictionary<Vector2Int, int>();

    private GameManager _gameManager;

    [SerializeField]
    private UnitsLists _unitList;
    public bool isSeparate;
    private List<Vector2Int> _listSeparateUnits = new List<Vector2Int>();
    private AnimationDesth _animationDesth;

    private bool _isTargeting;
    private Action<MapObject> _actionToExecute;

    private bool _isConfirmingAttack = false;

    private ProjectileController _baseProjectile;

    public static MapManager _instance;
    public static MapManager Instance
    {
        get
        {
            if (_instance == null)
            {
                Debug.LogError("MapManager is null");
            }
            return _instance;
        }
    }

    bool isClickRepeated = false;

    private void Awake()
    {
        _instance = this;
        isSeparate = false;
        
    }
    private void Start()
    {
        _animationDesth = AnimationDesth.Instance;
        _baseProjectile = transform.Find("BaseProjectile").GetComponent<ProjectileController>();
        _baseProjectile.gameObject.SetActive(false);
    }
    public void SetMap(string map)
    {
        this.map = MapLoader.LoadMap(Application.streamingAssetsPath + "/Maps/"+map+ ".csv");
    }
    public void SetMap(SavedGame save)
    {
        this.map = MapLoader.LoadMapFromSave(save);
    }
    public void LoadMap()
    {
        _gameManager = FindObjectOfType<GameManager>();

        if (map == null) SetMap("Island I");

        gameObject.GetComponent<MapGenerator>().GenerateMap(map);

        foreach (var i in map.getAllUnits())
            _addUnitGraphic(i.Key, i.Value);

        foreach (var i in map.getAllBuildings())
            _addBuildingGraphic(i.Key, i.Value);
        
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && !UIDetector.BlockedByUI)
        {
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = 10;

            Vector3 globalMousePosition = Camera.main.ScreenToWorldPoint(mousePos);

            Vector2Int mouseMapCoordinates = new Vector2Int((int)Mathf.Round(globalMousePosition.x), -(int)Mathf.Round(globalMousePosition.y));

            _select(mouseMapCoordinates);
        }
    }

    private void _select(Vector2Int pos)
    {
        //checks if click is repeated
        if (pos == _selectedTile)
            isClickRepeated = !isClickRepeated;
        else
            isClickRepeated = false;

        //

        if (FogOfWarManager.Instance.fogOfWar.GetPixel(pos.x, pos.y).r > 0.5) return;

        if (_isTargeting)
        {
            if (map.getUnit(pos) == null && map.getBuilding(pos) == null) return;
            _actionToExecute(map.getUnit(pos));
            _isTargeting = false;
            _tryEnableActionMenu();
            return;
        }


        MapObject selectedObject = null;
        if (pos.x < 0 || pos.x > map.tiles.GetLength(0) - 1 ||
            pos.y < 0 || pos.y > map.tiles.GetLength(1) - 1)
            return;
        HealthbarView();
        _selectedTile = pos;
        if (isSeparate && _listSeparateUnits.Contains(pos) && _listSeparateUnits[0] !=pos)
        {
            _selectionType = 7;
            isSeparate = false;
        }
        //else if (isSeparate && !listEnemyUnits.Contains(pos) && _listSeparateUnits.Count == 0)
        //{
        //    _selectionType = 6;
        //}
        else if (listEnemyUnits.Contains(pos))
        {
            if (map.getUnit(pos) != null)
            {
                if (map.getUnit(pos).teamID != _gameManager.currentTeam)
                {
                    if (_isConfirmingAttack)
                    {
                        Attack(pos);
                        _isConfirmingAttack = false;
                        _selectionMarker.sprite = _unitMarker;
                        return;
                    }
                    _selectionType = 5;          // Attack Unit
                }
            }
            else if (map.getBuilding(pos) != null)
            {
                if (map.getBuilding(pos).teamID != _gameManager.currentTeam)
                {
                    if (_isConfirmingAttack)
                    {
                        Attack(pos);
                        _isConfirmingAttack = false;
                        _selectionMarker.sprite = _unitMarker;
                        return;
                    }
                    _selectionType = 5;          // Attack Building
                }
            }

        }
        else if (listPositions.Contains(pos) && (!isClickRepeated || map.getBuilding(pos) == null)) _selectionType = 4;                                   // Movement Unit
        else if (listStackingUnits.Contains(pos)) _selectionType = 4;                               // Movement Unit
        else if (map.getUnit(pos) != null && map.getUnit(pos).teamID == _gameManager.currentTeam && (!isClickRepeated || map.getBuilding(pos) == null))   // Units
        {
            _selectionType = 3; 
            selectedObject = map.getUnit(pos); 
        }        
        else if (map.getUnit(pos) != null && map.getUnit(pos).teamID != _gameManager.currentTeam) _selectionType = 2;        // Units Enemy
        else if (map.getBuilding(pos) != null && (map.getBuilding(pos).teamID == _gameManager.currentTeam || map.getBuilding(pos).pounding <= 0)) { _selectionType = 1; selectedObject = map.getBuilding(pos); }// Buildings
        else _selectionType = 0;                                                                                              // Tiles
        _selectionMarker.gameObject.SetActive(true);

        _selectionMarker.transform.LeanMove(new Vector3(pos.x, -pos.y, -1), 0.1f);
        LeanTween.scale(_selectionMarker.gameObject, new Vector3(0.5f, 0.5f, 0.5f), 0.05f).setOnComplete(
            () => LeanTween.scale(_selectionMarker.gameObject, new Vector3(1f, 1f, 1f), 0.05f));
        //Debug.Log("To powinno by� 4!");
        if (!new int[]{ 3, 4, 5, 6, 7 }.Contains(_selectionType))
        {
            _unDrawMarkers();
        }

        _isConfirmingAttack = false;

        Debug.Log("Test select" + _selectionType);
        if (!new int[] { 3, 6, 7 }.Contains(_selectionType)) _unitList.ClearList();
        if (listStackingUnits.Contains(pos)) _selectionType = 4;  //Spojrzeć tutaj bo coś nie dobrego się dzieje, wyżej ten sam warunek nie przechodzi!
        switch (_selectionType)
        {
            case 0: _selectionMarker.sprite = _tileMarker; /*Debug.Log("Selected a Tile(" + map.tiles[pos.x, pos.y] + ") at position " + pos);*/ break; //tiles
            case 1: _selectionMarker.sprite = _unitMarker; /*Debug.Log("Selected a Building(" + map.getBuilding(pos).name + ") at position " + pos);*/ break; //buildings
            case 2: _selectionMarker.sprite = _unitMarker; Healthbar healthbar = _unitRenderers.GetValueOrDefault(pos).gameObject.GetComponentsInChildren<Healthbar>()[0];
                Canvas canvas = healthbar.gameObject.GetComponents<Canvas>()[0];
                canvas.sortingOrder = 0; break; //units
            case 3: _selectionMarker.sprite = _unitMarker; /*Debug.Log("Selected a Unit(" + map.getUnit(pos).name + ") at position " + pos);*/ 
                _getUnitMovementOptions(pos, map.getUnit(pos).moveRange, map.getUnit(pos).attackRange, true);
                _getUnitAttackOptions(pos);
                Healthbar healthbarr = _unitRenderers.GetValueOrDefault(pos).gameObject.GetComponentsInChildren<Healthbar>()[0];
                Canvas canvass = healthbarr.gameObject.GetComponents<Canvas>()[0];
                _unitList.SetUnit(map.getUnit(pos));
                _unitList.RefreshUnitsList();
                canvass.sortingOrder = 0; break; //units
            case 4: _selectionMarker.sprite = _unitMarker; Move(pos); break; //move
            case 5: _selectionMarker.sprite = _confirmationMarker; _isConfirmingAttack = true; break;//Attack(pos); break; //attack
            //case 6: _selectionMarker.sprite = _unitMarker; _getUnitSeparateMovementOptions(listPositions[0], 1); break; //separate units
            case 7: _selectionMarker.sprite = _unitMarker; _CreateSeparateUnit(pos); break; //separate movement units
        }

        if (selectedObject != null)
        {
            ActionMenuManager.Instance.LoadMapObject(selectedObject);
            _tryEnableActionMenu();
        } 
        else
        {
            ActionMenuManager.Instance.HideMenu();
        }
        
    }
    private void _drawMarkers(Vector2Int pos, int markerID = 0)
    {
        switch (markerID)
        {
            case 0:
                _drawPositionUnit.sprite = _positionOpt;
                break;
            case 1:
                _drawPositionUnit.sprite = _attackOpt;
                break;
            case 2:
                _drawPositionUnit.sprite = _splitOpt;
                break;
        }
        _drawPositionUnit.transform.position = new Vector3(pos.x, -pos.y, -2);
        
        GameObject g = Instantiate(_drawPositionUnit.gameObject, _drawPositionUnit.transform.position , Quaternion.identity).gameObject ;
        g.SetActive(true);
        g.GetComponent<idTile>().id=drawLines.Count;
        drawLines.Add(g);

    }
    private void _unDrawMarkers()
    {
        for (int i = drawLines.Count-1; i >= 0 ; i--)
        {
            Destroy(drawLines[i]);
            drawLines.RemoveAt(i);
            
        }
        for (int i = listPositions.Count - 1; i >= 0; i--)
        {
            //rangesDict.Remove(listPositions[i]);
            listPositions.RemoveAt(i);
        }
        rangesDict.Clear();
        for (int i = listEnemyUnits.Count-1; i >= 0; i--)
        {
            listEnemyUnits.RemoveAt(i);
        }
        for (int i = listStackingUnits.Count - 1; i >= 0; i--)
        {
            listStackingUnits.RemoveAt(i);
        }
        for (int i = _listSeparateUnits.Count - 1; i >= 0; i--)
        {
            _listSeparateUnits.RemoveAt(i);
        }
        

    }
    //Healthbar view
    public void HealthbarView()
    {
        Dictionary<Vector2Int, SpriteRenderer>.ValueCollection temp = _unitRenderers.Values;
        foreach (SpriteRenderer i in temp)
        {
            Healthbar healthbar = i.gameObject.GetComponentsInChildren<Healthbar>()[0];
            Canvas canvass = healthbar.gameObject.GetComponents<Canvas>()[0];
            canvass.sortingOrder = -10;
        }
        
    }

    // Attack Function
    public void Attack(Vector2Int pos)
    {
        bool isFlip=false;
        bool isSameFlip = false;
        Unit attacker = map.getUnit(listPositions[0]);
        Unit defender;
        defender = map.getUnit(pos);
        if (listPositions[0].x > pos.x)
        {
            isFlip = true;
        }
        else if (listPositions[0].x < pos.x)
        {
            isFlip = false;
        }
        else
        {
            isSameFlip = true;
        }
        Animator anim = attacker.animator;
        if (map.getBuilding(pos) != null && map.getUnit(pos) == null)
        {
            if (!isSameFlip)
            {
                anim.gameObject.GetComponent<SpriteRenderer>().flipX = isFlip;
            }
            else
            {
                anim.gameObject.GetComponent<SpriteRenderer>().flipX = isFlip;
            }
            
            anim.SetTrigger("Attack");
            map.getBuilding(pos).DealDamageBuilding(attacker.getAttackStrengthBuilding(false));

            map.getUnit(listPositions[0]).attackRange = 0;
            //map.getUnit(listPositions[0]).moveRange = 0;
            _unDrawMarkers();
            Debug.Log("Percent Pounding: " + map.getBuilding(pos).pounding);
        }
        if (map.getUnit(pos) != null)
        {
            Animator animator = defender.animator;
            if (!isSameFlip)
            {
                anim.gameObject.GetComponent<SpriteRenderer>().flipX = isFlip;
                animator.gameObject.GetComponent<SpriteRenderer>().flipX = !isFlip;
            }
            else
            {
                anim.gameObject.GetComponent<SpriteRenderer>().flipX = isFlip;
                animator.gameObject.GetComponent<SpriteRenderer>().flipX = isFlip;
            }
            if (attacker.attackRange > 1)
            {
                Projectile projectile = UnitDataHolder.Instance.unitData.GetValueOrDefault(attacker.ID).projectile;

                if (projectile != null)
                {
                    SendProjectile(projectile, listPositions[0], pos);
                }
            }

            if (map.getBuilding(pos) != null)
            {
                anim.SetTrigger("Attack");
                animator.SetTrigger("Attack");
                map.AttackUnit(pos, listPositions[0], true);
                map.getBuilding(pos).DealDamageBuilding(map.getUnit(listPositions[0]).getAttackStrengthBuilding(true));
            }
            else
            {
                if (map.getUnit(pos).isTarget)
                {

                    anim.SetTrigger("Attack");
                    animator.SetTrigger("Attack");
                    map.AttackUnit(pos, listPositions[0], false);
                    Debug.Log("To chce zobaczyć po Finished");

                }

            }

            if (attacker.getAppliedStatusEffects().Count > 0)
            {
                defender.applyStatusEffects(attacker.getAppliedStatusEffects());

                Debug.Log("fx applied");
            }

            RefreshHealthbar(pos);
            RefreshHealthbar(listPositions[0]);
            if (/*map.getUnit(pos).getHP() < 0 &&*/ map.getUnit(pos).numberOfUnits < 1)
            {
                if (map.getUnit(pos).GetUnitType() is Unit.UnitType.god)
                {
                    _gameManager.GetCurrentTeamObj().isGodRespawned = false;
                }
                DeleteUnitGraphic(pos);
                map.RemoveUnit(pos);
                _animationDesth.StartAnimation(pos);
            }
            if (map.getUnit(listPositions[0]).numberOfUnits < 1)
            {
                DeleteUnitGraphic(listPositions[0]);
                map.RemoveUnit(listPositions[0]);
                _animationDesth.StartAnimation(listPositions[0]);
            }
            _unDrawMarkers();
        }
        
    }

    public void RefreshHealthbar(Vector2Int pos)
    {
        Healthbar healthbar = _unitRenderers.GetValueOrDefault(pos).gameObject.GetComponentsInChildren<Healthbar>()[0];
        healthbar.SetHealth(map.getUnit(pos).getHP());
        healthbar.SetNumberOfUnits(map.getUnit(pos).numberOfUnits);
    }
    // Move Function And Occupation
    public void Move(Vector2Int pos)
    {
        bool isFlip=false;
        if (listPositions[0].x > pos.x)
        {
            isFlip = true;
        }
        else if (listPositions[0].x < pos.x)
        {
            isFlip = false;
        }

        if (pos == listPositions[0] && map.getBuilding(pos)!=null)
        {
            if (map.getBuilding(pos).teamID != _gameManager.currentTeam)
            {
                map.OccupationUnit(pos);
                _unDrawMarkers();
            }
            else return;
            
        }
        else if (pos != listPositions[0] && listStackingUnits.Contains(pos))
        {
            if((map.getUnit(pos).numberOfUnits+ map.getUnit(listPositions[0]).numberOfUnits) > 30)
            {
                AlertManager.AddAlert("Too many units! Limit 30 units.");
                return;
            }
            map.getUnit(pos).numberOfUnits += map.getUnit(listPositions[0]).numberOfUnits;
            map.getUnit(pos).numberOfUnits--;
            int hp = map.getUnit(pos).getHP() + map.getUnit(listPositions[0]).getHP();
            while(hp> map.getUnit(pos).getBaseHP())
            {
                map.getUnit(pos).numberOfUnits++;
                hp -= map.getUnit(pos).getBaseHP();
            }
            if (hp <= map.getUnit(pos).getBaseHP())
            {
                map.getUnit(pos).SetHP(hp);
            }
            int range;
            if (map.getUnit(pos).moveRange <= 0)
                range = 0;
            else
                range = map.getUnit(pos).moveRange - (Mathf.Abs(listPositions[0].x - pos.x) + Mathf.Abs(listPositions[0].y - pos.y));
            if (range <= 0) range = 0;
            if (range < map.getUnit(listPositions[0]).moveRange)   
            {
                map.getUnit(pos).moveRange = range;
            }
            if(map.getUnit(pos).attackRange > map.getUnit(listPositions[0]).attackRange)
            {
                map.getUnit(pos).attackRange = map.getUnit(listPositions[0]).attackRange;
            }   
            DeleteUnitGraphic(listPositions[0]);
            map.RemoveUnit(listPositions[0]);
            RefreshHealthbar(pos);
            _unDrawMarkers();
        }
        else if (pos != listPositions[0] && map.getUnit(pos)==null)
        {
            
            _moveUnitGraphic(listPositions[0], pos,isFlip);            
            map.MoveUnit(listPositions[0], pos, rangesDict.GetValueOrDefault(pos));
           
            _unDrawMarkers();
        }
        else if (pos == listPositions[0]) return;

        onUnitMoved();
    }

    public event Action onUnitMoved;

    //a function that traces a path for unit
    Building cachedBuildingAtPosition;
    Unit cachedUnitAtPosition;
    private void _getUnitMovementOptions(Vector2Int pos, int range, int rangeAttack, bool isFirstTime)
    {
        cachedBuildingAtPosition = map.getBuilding(pos);
        cachedUnitAtPosition = map.getUnit(pos);

        if (isFirstTime)
        {
            listPositions.Clear();
            rangesDict.Clear();
            _unDrawMarkers();
            listPositions.Add(pos);
            rangesDict.Add(pos, range);
            if (range == 0) return;
            if (cachedBuildingAtPosition != null)
            {
                if (cachedBuildingAtPosition.teamID != _gameManager.currentTeam) _drawMarkers(pos);
            }
        }
        else
        {
            if (cachedUnitAtPosition != null && rangeAttack >= 0 && cachedUnitAtPosition.teamID != _gameManager.currentTeam)
            {
                //_drawMarkers(pos, 1);
                //listEnemyUnits.Add(pos);
                return;
            }
            else if(cachedBuildingAtPosition != null && rangeAttack >= 0)
            {
                if (cachedBuildingAtPosition.teamID == _gameManager.currentTeam || (cachedBuildingAtPosition.teamID != _gameManager.currentTeam && cachedBuildingAtPosition.pounding <= 0))
                {
                    _drawMarkers(pos);
                    listPositions.Add(pos);
                    rangesDict.TryAdd(pos, range);
                }
                else if (cachedBuildingAtPosition.pounding > 0)
                {
                    //_drawMarkers(pos, 1);
                    //listEnemyUnits.Add(pos);
                    return;
                }
            }
            else if(cachedUnitAtPosition != null && cachedUnitAtPosition.teamID == _gameManager.currentTeam && cachedUnitAtPosition.GetUnitType()== map.getUnit(listPositions[0]).GetUnitType() && listPositions[0]!=pos && cachedUnitAtPosition.name== map.getUnit(listPositions[0]).name)
            {
                listStackingUnits.Add(pos);
                return;
            }
            else if (!map.isTileNotToGo(pos, map.getUnit(_selectedTile)) && cachedUnitAtPosition == null && !listPositions.Contains(pos))
            {
                
                _drawMarkers(pos);
                 listPositions.Add(pos);
                 rangesDict.Add(pos, range);
            }
            else
            {
                if (listPositions.Contains(pos) && rangesDict.GetValueOrDefault(pos) < range - 1)
                {
                    rangesDict.Remove(pos);
                    rangesDict.Add(pos, range);
                    goto recurency;
                }
                return;
            }
        }
        recurency:
        if (range == 0)
        {
            return;
        }
        
        _getUnitMovementOptions(new Vector2Int(pos.x, pos.y - 1), range - 1, rangeAttack-1, false); //Up
        _getUnitMovementOptions(new Vector2Int(pos.x + 1, pos.y), range - 1, rangeAttack - 1, false); //Right
        _getUnitMovementOptions(new Vector2Int(pos.x, pos.y + 1), range - 1, rangeAttack - 1, false); //Down
        _getUnitMovementOptions(new Vector2Int(pos.x - 1, pos.y), range - 1, rangeAttack - 1, false); //Left

    }

    private void _getUnitAttackOptions(Vector2Int unitPosition)
    {
        int range = map.getUnit(unitPosition).attackRange;

        //looping through all the tiles in range
        for (int x = unitPosition.x - range; x <= unitPosition.x + range; x++)
        {
            for (int y = unitPosition.y - range; y <= unitPosition.y + range; y++)
            {
                if (x >= 0 && x < map.tiles.GetLength(0) && y >= 0 && y < map.tiles.GetLength(1) &&
                    Math.Abs(x - unitPosition.x) + Math.Abs(y - unitPosition.y) <= range &&
                    FogOfWarManager.Instance.fogOfWar.GetPixel(x, y).g < 0.5)
                {
                    Vector2Int pos = new Vector2Int(x, y);
                    Unit unitAtPosition = map.getUnit(pos);
                    Building buildingAtPosition = map.getBuilding(pos);


                    //checking if object is attackable
                    if (unitAtPosition != null && unitAtPosition.teamID != _gameManager.currentTeam)
                    {
                        _drawMarkers(pos, 1);
                        listEnemyUnits.Add(pos);
                    }
                    else if (buildingAtPosition != null && buildingAtPosition.teamID != _gameManager.currentTeam && buildingAtPosition.pounding > 0)
                    {
                        _drawMarkers(pos, 1);
                        listEnemyUnits.Add(pos);
                    }
                }
            }
        }
    }

    public void _getUnitSeparateMovementOptions(Vector2Int pos, int range, Vector2Int startPos, bool isFirstTime=false )
    {
        if (isFirstTime)
        {
            _unDrawMarkers();
            _listSeparateUnits.Add(pos);
        }
        else if (map.getBuilding(pos) != null)
        {
            if (map.getBuilding(pos).teamID == _gameManager.currentTeam || (map.getBuilding(pos).teamID != _gameManager.currentTeam && map.getBuilding(pos).pounding <= 0))
            {
                _drawMarkers(pos, 2);
                _listSeparateUnits.Add(pos);
            }
            else
            {
                return;
            }
        }
        else if (map.getUnit(pos) != null && map.getUnit(pos).teamID == _gameManager.currentTeam && map.getUnit(pos).GetUnitType() == map.getUnit(startPos).GetUnitType() && map.getUnit(pos).ID==map.getUnit(startPos).ID)
        {
            _drawMarkers(pos, 2);
            _listSeparateUnits.Add(pos);
        }
        else if (!map.isTileNotToGo(pos, map.getUnit(_selectedTile)) && map.getUnit(pos) == null)
        {
            _drawMarkers(pos, 2);
            _listSeparateUnits.Add(pos);
        }
        else
        {
            return;
        }
        

        if (range == 0)
        {
            return;
        }
        _getUnitSeparateMovementOptions(new Vector2Int(pos.x, pos.y - 1), range - 1, startPos); //Up
        _getUnitSeparateMovementOptions(new Vector2Int(pos.x + 1, pos.y), range - 1, startPos); //Right
        _getUnitSeparateMovementOptions(new Vector2Int(pos.x, pos.y + 1), range - 1, startPos); //Down
        _getUnitSeparateMovementOptions(new Vector2Int(pos.x - 1, pos.y), range - 1, startPos); //Left

    }

    public void ResetDrawEndTurn()
    {
        _unDrawMarkers();
        _selectionType = 0;
        _selectionMarker.sprite = null;
        _selectedTile = new Vector2Int(-1, 1);
    }

    private void _CreateSeparateUnit(Vector2Int pos)
    {

        Unit unit = _unitList.GetUnit();
        Unit temp;
        if (!map.getAllUnits().Keys.Contains(pos))
        {
            temp = Instantiate(UnitDataHolder.Instance.unitData.GetValueOrDefault(unit.ID).unit).Init();
            
        }
        else
        {
            temp = map.getUnit(pos);
            if ((unit.numberOfUnits+temp.numberOfUnits) > 30)
            {
                AlertManager.AddAlert("Too many units! Limit 30 units.");
                return;
            }
        }

        
        temp.teamID = GameManager.Instance.currentTeam;
        
        AddUnitToMap(_selectedTile, temp);
        temp.numberOfUnits--;
        _unitList.SetNewUnit(temp);
        RefreshHealthbar(pos);
        RefreshHealthbar(_listSeparateUnits[0]);
        _unDrawMarkers();
        ActionMenuManager.Instance.HideMenu();
    }
    public void TransformationWerewolf(Vector2Int pos, Unit unit, bool werewolf)
    {
        DeleteUnitGraphic(pos);
        if (werewolf)
        {
            unit.ID = "werewolf";
        }
        else
        {
            unit.ID = "warrior";
        }
        
        _addUnitGraphic(pos, unit);

    }
    //managing unit graphics
    private void _addUnitGraphic(Vector2Int pos, Unit unit, bool isFlip = false, bool haveMoreAnimations = false)
    {
        
        SpriteRenderer sr = Instantiate(_baseRenderer.gameObject, new Vector3(pos.x, -pos.y, -1f), Quaternion.identity).GetComponent<SpriteRenderer>();
        sr.sprite = UnitDataHolder.Instance.unitData.GetValueOrDefault(unit.ID).sprite;
        //Debug.Log(isFlip);
        sr.flipX = isFlip;
        if (haveMoreAnimations)
        {
            Transform sourceTransform = unit.animator.gameObject.transform;
            Transform destinationTransform = sr.gameObject.transform;
            foreach (Transform child in sourceTransform)
            {
                if (child.gameObject.name == "Healthbar" || child.gameObject.name == "TeamIndicator") continue;
                child.SetParent(destinationTransform, true);
            }
        }
        Animator anim = sr.gameObject.GetComponent<Animator>();
        anim.runtimeAnimatorController= UnitDataHolder.Instance.unitData.GetValueOrDefault(unit.ID).anim;
        unit.animator = anim;
        Healthbar healthbar = sr.gameObject.GetComponentsInChildren<Healthbar>()[0];
        healthbar.SetMaxHealth(unit.getBaseHP());
        healthbar.SetHealth(unit.getHP());
        healthbar.SetNumberOfUnits(unit.numberOfUnits);
        ChangeIndicatorColor(sr, Color.green);
        sr.gameObject.SetActive(true);
        _unitRenderers.TryAdd(pos, sr);
        
    }

    public void DeleteUnitGraphic(Vector2Int pos)
    {
        Destroy(_unitRenderers.GetValueOrDefault(pos).gameObject);
        _unitRenderers.Remove(pos);
    }

    private void _moveUnitGraphic(Vector2Int from, Vector2Int to, bool isFlip=false)
    {
        bool temp = isFlip;
        Unit unit = map.getUnit(from);
        Color color = GetUnitGraphic(from).color;
        if (unit.animator.gameObject.transform.childCount > 2)
        {
            LeanTween.move(_unitRenderers.GetValueOrDefault(from).gameObject, new Vector3(to.x, -to.y, -1f), 0.2f)
            .setOnComplete(() => {
                _addUnitGraphic(to, unit, temp, true);
                GetUnitGraphic(to).color = color;
                DeleteUnitGraphic(from);
            }).setEaseInOutQuad();
        }
        else
        {
            LeanTween.move(_unitRenderers.GetValueOrDefault(from).gameObject, new Vector3(to.x, -to.y, -1f), 0.2f)
            .setOnComplete(() => {
                _addUnitGraphic(to, unit, temp);
                GetUnitGraphic(to).color = color;
                DeleteUnitGraphic(from);
            }).setEaseInOutQuad();
        }
        
    }

    //managing building graphics
    private void _addBuildingGraphic(Vector2Int pos, Building building)
    {
        SpriteRenderer sr = Instantiate(_baseRenderer.gameObject, new Vector3(pos.x, -pos.y, -0.5f), Quaternion.identity).GetComponent<SpriteRenderer>();
        Sprite[] sprites = BuildingDataHolder.Instance.buildingData.GetValueOrDefault(building.ID).sprite;
        if(sprites.Length <= 1)
        {
            sr.sprite = BuildingDataHolder.Instance.buildingData.GetValueOrDefault(building.ID).sprite[0];
        }
        else
        {
            SpriteAnimator sa = sr.AddComponent<SpriteAnimator>();
            sa.frames = sprites.ToList();
            sa.miliseconds = 1000/2;
        }
        ChangeIndicatorColor(sr, Color.green);
        sr.gameObject.SetActive(true);
        _buildingRenderers.Add(pos, sr);
    }

    private void _deleteBuildingGraphic(Vector2Int pos)
    {
        Destroy(_buildingRenderers.GetValueOrDefault(pos).gameObject);
        _buildingRenderers.Remove(pos);
    }

    private void _moveBuildingGraphic(Vector2Int from, Vector2Int to)
    {
        _addBuildingGraphic(to, map.getBuilding(from));
        _deleteBuildingGraphic(from);
    }

    public SpriteRenderer GetUnitGraphic(Vector2Int pos)
    {
        return _unitRenderers.GetValueOrDefault(pos);
    }

    private void _tryEnableActionMenu()
    {
        if (!ActionMenuManager.Instance.isOpen)
        {
            ActionMenuManager.Instance.ShowMenu();
        }
    }

    public void AddUnitToMap(Vector2Int pos, Unit unit)
    {
        if (map.getUnit(pos) != null)
        {
            if ((map.getUnit(pos).numberOfUnits + 1) > 30)
            {
                AlertManager.AddAlert("Too many units! Limit 30 units.");
                return;
            }
            if (unit.GetUnitType() is Unit.UnitType.god && _gameManager.GetCurrentTeamObj().isGodRespawned)
            {
                AlertManager.AddAlert("Limit 1 god!");
                return;
            }
            SpriteRenderer sr = GetUnitGraphic(pos);
            Healthbar healthbar = sr.gameObject.GetComponentsInChildren<Healthbar>()[0];
            healthbar.AddNumberOfUnits(1);
            map.getUnit(pos).numberOfUnits = healthbar.currentUnits;
            //map.getUnit(pos)
        }
        else
        {
            if (unit.GetUnitType() is Unit.UnitType.god && !_gameManager.GetCurrentTeamObj().isGodRespawned)
            {
                _gameManager.GetCurrentTeamObj().isGodRespawned = true;
            }
            else if(unit.GetUnitType() is Unit.UnitType.god && _gameManager.GetCurrentTeamObj().isGodRespawned)
            {
                AlertManager.AddAlert("Limit 1 god!");
                return;
            }
            map.AddUnit(pos, unit);
            _addUnitGraphic(pos, unit);
            
        }
        onUnitMoved();
    }

    public void ExecuteOnTarget(Action<MapObject> actionToExecute)
    {
        ActionMenuManager.Instance.HideMenu();
        _isTargeting = true;
        _actionToExecute = actionToExecute;
    }

    public Dictionary<Vector2Int, SpriteRenderer> GetAllUnitRenderers()
    {
        return _unitRenderers;
    }

    public Dictionary<Vector2Int, SpriteRenderer> GetAllBuildingRenderers()
    {
        return _buildingRenderers;
    }

    public List<Vector2Int> GetListPositions()
    {
        return listPositions;
    }

    public void DeleteBuilding(Vector2Int pos)
    {
        _deleteBuildingGraphic(pos);
        map.RemoveBuilding(pos);
    }

    public void AddBuildingToMap(Vector2Int pos, Building building)
    {
        map.AddBuilding(pos, building);
        _addBuildingGraphic(pos, building);
    }

    public void ChangeIndicatorColor(SpriteRenderer spriteRenderer, Color color)
    {
        spriteRenderer.transform.Find("TeamIndicator").GetComponent<SpriteRenderer>().color = color;
    }

    public void RefreshAllIndicatorColors()
    {
        foreach (var kvp in GetAllUnitRenderers())
        {
            if (map.getUnit(kvp.Key).teamID == _gameManager.GetCurrentTeam())
            {
                ChangeIndicatorColor(kvp.Value, Color.green);
            }
            else if (map.getUnit(kvp.Key).teamID == -2)
            {
                ChangeIndicatorColor(kvp.Value, Color.cyan);
            }
            else
            {
                ChangeIndicatorColor(kvp.Value, Color.red);
            }
        }

        foreach (var kvp in GetAllBuildingRenderers())
        {
            if (map.getBuilding(kvp.Key).teamID == _gameManager.GetCurrentTeam())
            {
                ChangeIndicatorColor(kvp.Value, Color.green);
            }
            else if (map.getBuilding(kvp.Key).teamID == -2)
            {
                ChangeIndicatorColor(kvp.Value, Color.cyan);
            }
            else
            {
                ChangeIndicatorColor(kvp.Value, Color.red);
            }
        }
    }
    
    public void SpawnMonsters(int amount)
    {
        string[] normalMonsterPool = { "basilics", "bear", "bies", "griffin", "ladyMidday", "leshy", "werewolf", "witch", "wolf" };
        string[] waterMonsterPool = { "rusalka", "utopiec" };

        while(amount > 0)
        {
            string idOfUnitToSpawn;
            Vector2Int targetPosition = new Vector2Int();
            do
            {
                targetPosition.x = UnityEngine.Random.Range(0, map.tiles.GetLength(0));
                targetPosition.y = UnityEngine.Random.Range(0, map.tiles.GetLength(1));
            }
            while (new string[] { "g", "l" }.Contains(map.tiles[targetPosition.x, targetPosition.y]) || map.getBuilding(targetPosition) != null || map.getUnit(targetPosition) != null);

            if (new string[] { "o", "r" }.Contains(map.tiles[targetPosition.x, targetPosition.y]))
            {
                idOfUnitToSpawn = waterMonsterPool[UnityEngine.Random.Range(0, waterMonsterPool.Length)];
            }
            else
            {
                idOfUnitToSpawn = normalMonsterPool[UnityEngine.Random.Range(0, normalMonsterPool.Length)];
            }
            
            Unit temp = Instantiate(UnitDataHolder.Instance.unitData.GetValueOrDefault(idOfUnitToSpawn).unit).Init();
            temp.teamID = -1;
            int chance = UnityEngine.Random.Range(0, 100);
            if((chance > 10 && chance<25) || (chance >50 && chance <67) || chance == 99)
            {
                temp.numberOfUnits = UnityEngine.Random.Range(1, 4);
            }
            AddUnitToMap(targetPosition, temp);
            Debug.Log(idOfUnitToSpawn + " respi");
            amount--;
        }
    }

    public void SendProjectile(Projectile projectile, Vector2Int from, Vector2Int to)
    {
        ProjectileController temp = Instantiate(_baseProjectile);

        temp.gameObject.SetActive(true);

        temp.SetUpProjectile(
            projectile,
            new Vector3(from.x, -from.y, -2),
            new Vector3(to.x, -to.y, -2)
            );

        Debug.Log("Strzała wysłana");
    } 

    public void AddAnimationToUnit(Vector2Int pos, string boolName)
    {
        GameObject newGameObject = new GameObject("AnimationPlayer");
        Animator animator = newGameObject.AddComponent<Animator>();
        SpriteRenderer sr = newGameObject.AddComponent<SpriteRenderer>();
        animator.runtimeAnimatorController = Instantiate(Resources.Load<RuntimeAnimatorController>("Effects"));
        animator.keepAnimatorControllerStateOnDisable = true;
        animator.SetBool(boolName, true);
        newGameObject.transform.SetParent(_unitRenderers.GetValueOrDefault(pos).transform);
        newGameObject.transform.localPosition = new Vector3(0, 0, -2);
        map.getUnit(pos).statusEffectDisplays.Add(boolName, newGameObject);
    }

    public void RefreshAllHealthbars()
    {
        foreach(KeyValuePair<Vector2Int, SpriteRenderer> kvp in _unitRenderers)
        {
            RefreshHealthbar(kvp.Key);
        }
    }
}
