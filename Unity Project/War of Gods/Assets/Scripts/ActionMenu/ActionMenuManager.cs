using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ActionMenuManager : MonoBehaviour
{
    private RectTransform _rectTransform;

    //Icon
    private Image _actionIcon;

    //Description
    private TextMeshProUGUI _actionName;
    private TextMeshProUGUI _actionDescription;

    private TextMeshProUGUI _parameter1AmountText;
    private TextMeshProUGUI _parameter2AmountText;
    private TextMeshProUGUI _parameter3AmountText;
    private TextMeshProUGUI _parameter4AmountText;

    private Image _parameter1AmountImage;
    private Image _parameter2AmountImage;
    private Image _parameter3AmountImage;
    private Image _parameter4AmountImage;

    //Button
    private RectTransform _baseActionOption;
    private Image _actionOptionIcon;

    [SerializeField]
    private resourceSprites[] _resourceIconSprites = new resourceSprites[2];

    private RectTransform _tick;

    private List<GameObject> _instancedButtons = new List<GameObject>();

    //State Stuff
    public bool isOpen = false;

    private string _selectedAction;

    private MapObject _selectedObject;

    //map data
    private MapManager _mapManager;

    [SerializeField]
    private GameObject _unitList;

    //Singleton Stuff
    private static ActionMenuManager _instance;

    public static ActionMenuManager Instance
    {
        get
        {
            if (_instance == null)
            {
                Debug.LogError("ActionMenuManager is null");
            }
            return _instance;
        }
    }

    private void Awake()
    {
        _instance = this;
        _mapManager = FindObjectOfType<MapManager>();
    }

        [System.Serializable]
    private struct resourceSprites
    {
        public Sprite resource1Sprite;
        public Sprite resource2Sprite;
        public Sprite resource3Sprite;
        public Sprite resource4Sprite;
    }

    // Start is called before the first frame update
    void Start()
    {
        _rectTransform = GetComponent<RectTransform>();

        _actionIcon = transform.Find("IconBackground/ActionIcon").GetComponent<Image>();

        _actionName = transform.Find("DescriptionBackground/ActionName").GetComponent<TextMeshProUGUI>();
        _actionDescription = transform.Find("DescriptionBackground/ActionDescription").GetComponent<TextMeshProUGUI>();

        _parameter1AmountText = transform.Find("DescriptionBackground/Wood").GetComponentInChildren<TextMeshProUGUI>();
        _parameter2AmountText = transform.Find("DescriptionBackground/Gold").GetComponentInChildren<TextMeshProUGUI>();
        _parameter3AmountText = transform.Find("DescriptionBackground/Food").GetComponentInChildren<TextMeshProUGUI>();
        _parameter4AmountText = transform.Find("DescriptionBackground/Faith").GetComponentInChildren<TextMeshProUGUI>();

        _parameter1AmountImage = transform.Find("DescriptionBackground/Wood").GetComponentInChildren<Image>();
        _parameter2AmountImage = transform.Find("DescriptionBackground/Gold").GetComponentInChildren<Image>();
        _parameter3AmountImage = transform.Find("DescriptionBackground/Food").GetComponentInChildren<Image>();
        _parameter4AmountImage = transform.Find("DescriptionBackground/Faith").GetComponentInChildren<Image>();

        _baseActionOption = transform.Find("ActionOptionsBackground/ActionOption").GetComponent<RectTransform>();
        _actionOptionIcon = transform.Find("ActionOptionsBackground/ActionOption/ActionOptionIcon").GetComponent<Image>();

        _baseActionOption.gameObject.SetActive(false);

        _tick = transform.Find("ActionOptionsBackground/Tick").GetComponent<RectTransform>();

        _tick.gameObject.SetActive(false);
    }

    public void LoadMapObject(MapObject mapObject)
    {
        _selectedObject = mapObject;
        Sprite actionIconSprite = null;
        _selectedAction = mapObject.actions[0];

        if (mapObject is Unit)
        {
            actionIconSprite = UnitDataHolder.Instance.unitData.GetValueOrDefault(mapObject.ID).sprite;
        }
        else if (mapObject is Building)
        {
            actionIconSprite = BuildingDataHolder.Instance.buildingData.GetValueOrDefault(mapObject.ID).sprite[0];
        }

        _actionIcon.sprite = actionIconSprite;

        _generateActionButtons(mapObject);
        _openAction(_selectedAction);
    }

    private void _openAction(string actionID)
    {
        ActionData action = ActionDataHolder.Instance.actionData.GetValueOrDefault(actionID, null);
        if (actionID == "info")
            _loadInfo(_selectedObject);
        else if (action is PaymentActionData)
            _loadPaymentAction((PaymentActionData)action);
    }

    private void _onButtonPressed(string actionID)
    {
        if (actionID == _selectedAction && _selectedAction != "info")
        {
            ActionData action = ActionDataHolder.Instance.actionData.GetValueOrDefault(actionID, null);
            


            if (action is PaymentActionData)
            {
                if (((PaymentActionData)action).foodCost > GameManager.Instance.GetCurrentTeamObj().GetAmountFood() ||
                    ((PaymentActionData)action).goldCost > GameManager.Instance.GetCurrentTeamObj().GetAmountGold() ||
                    ((PaymentActionData)action).woodCost > GameManager.Instance.GetCurrentTeamObj().GetAmountWood() ||
                    ((PaymentActionData)action).faithCost > GameManager.Instance.GetCurrentTeamObj().GetAmountFaithPoints())
                {
                    Debug.Log("Not enough resources.");
                    AlertManager.AddAlert("Not enough resources.");
                    return;
                }
            }

            
            _executeActionCode(action);
            return;
        }
        _selectedAction = actionID;
        _openAction(actionID);
        _generateActionButtons(_selectedObject);
    }

    private void _generateActionButtons(MapObject mapObject)
    {
        for (int i = _instancedButtons.Count-1; i >= 0; i--)
        {
            Destroy(_instancedButtons[i]);
        }

        _instancedButtons.Clear();

        Vector3 lastPosition = _baseActionOption.transform.position;

        foreach(string actionID in mapObject.actions)
        {
            ActionData action = ActionDataHolder.Instance.actionData.GetValueOrDefault(actionID);
            _actionOptionIcon.sprite = action.icon;

            GameObject temp = Instantiate(_baseActionOption.gameObject);
            temp.transform.SetParent(_baseActionOption.transform.parent);
            temp.transform.position = lastPosition;

            temp.GetComponent<Button>().onClick.AddListener(() => _onButtonPressed(actionID));

            if (_selectedAction == actionID)
            {
                _tick.position = lastPosition;

                if (actionID == "info")
                    _tick.gameObject.SetActive(false);
                else
                    _tick.gameObject.SetActive(true);
            }
                

            lastPosition += new Vector3(96 + 8, 0, 0);

            if (lastPosition.x >= _baseActionOption.transform.position.x + (96 + 8) * 5)
                lastPosition = _baseActionOption.transform.position - new Vector3(0, 96 + 8, 0);

            temp.SetActive(true);

            _instancedButtons.Add(temp);
            _tick.SetAsLastSibling();
        }
    }

    private void _loadPaymentAction(PaymentActionData action)
    {
        _actionName.text = action.name;

        string desc = action.description;
        desc = parseCodes(desc);
        _actionDescription.text = desc;

        _setResourceSprites(0);
        _parameter1AmountText.text = action.woodCost.ToString();
        _parameter2AmountText.text = action.goldCost.ToString();
        _parameter3AmountText.text = action.foodCost.ToString();
        _parameter4AmountText.text = action.faithCost.ToString();
    }

    private string parseCodes(string input)
    {
        if (_selectedObject is Unit)
        {
            input = input.Replace("<damage>", ((Unit)_selectedObject).getAttackStrength().ToString());
        }
        
        return input;
    }

    private void _loadAction(ActionData action)
    {

    }

    private void _loadInfo(MapObject mapObject)
    {
        string title = mapObject.name + " Info";
        string description = "";

        if (mapObject is Unit)
        {
            description = UnitDataHolder.Instance.unitData.GetValueOrDefault(mapObject.ID).description;

            _setResourceSprites(1);

            _parameter1AmountText.text = ((Unit)mapObject).getHP() + "/" + ((Unit)mapObject).getBaseHP();
            _parameter2AmountText.text = ((Unit)mapObject).getAttackStrength().ToString();
            _parameter3AmountText.text = ((Unit)mapObject).moveRange+ "/" + ((Unit)mapObject).getBaseMoveRange();
            _parameter4AmountText.text = ((Unit)mapObject).attackRange.ToString();
            if (!_unitList.activeSelf)
            {
                _unitList.SetActive(true);
            }
        }
        else if (mapObject is Building)
        {
            if (_unitList.activeSelf)
            {
                _unitList.SetActive(false);
            }
            _setResourceSprites(0);
            description = BuildingDataHolder.Instance.buildingData.GetValueOrDefault(mapObject.ID).description;
            if (mapObject is ProductiveBuilding || mapObject is Base)
            {
                _parameter1AmountText.text = ((ProductiveBuilding)mapObject).GetProducedWood().ToString();
                _parameter2AmountText.text = ((ProductiveBuilding)mapObject).GetProducedGold().ToString();
                _parameter3AmountText.text = ((ProductiveBuilding)mapObject).GetProducedFood().ToString();
                _parameter4AmountText.text = ((ProductiveBuilding)mapObject).GetProducedFaith().ToString();
            } else
            {
                _parameter1AmountText.text = "0";
                _parameter2AmountText.text = "0";
                _parameter3AmountText.text = "0";
                _parameter4AmountText.text = "0";
            }
        }

        _actionName.text = title;

        description = parseCodes(description);

        _actionDescription.text = description;

        
    }

    public void ShowMenu()
    {
        LeanTween.moveY(gameObject, 175, 0.2f).setEaseInOutQuad();
    }

    public void HideMenu()
    {
        LeanTween.moveY(gameObject, -400, 0.2f).setEaseInOutQuad();
    }

    private void _setResourceSprites(int type)
    {
        _parameter1AmountImage.sprite = _resourceIconSprites[type].resource1Sprite;
        _parameter2AmountImage.sprite = _resourceIconSprites[type].resource2Sprite;
        _parameter3AmountImage.sprite = _resourceIconSprites[type].resource3Sprite;
        _parameter4AmountImage.sprite = _resourceIconSprites[type].resource4Sprite;
    }

    private void _executeActionCode(ActionData action)
    {
        string[] parsedCode = action.actionCode.Split(":");

        if (FogOfWarManager.Instance.fogOfWar.GetPixel(_selectedObject.position.x, _selectedObject.position.y).g > 0.5)
        {
            Debug.Log("Object is outside of range.");
            AlertManager.AddAlert("Object is outside of range.");
            return;
        }

        switch (parsedCode[0])
        {
            case "spawn":
                if ( _mapManager.map.getUnit(_mapManager._selectedTile) == null || 
                    (_mapManager.map.getUnit(_mapManager._selectedTile) != null && _mapManager.map.getUnit(_mapManager._selectedTile).ID == parsedCode[1]))
                {
                    Unit temp = Instantiate(UnitDataHolder.Instance.unitData.GetValueOrDefault(parsedCode[1]).unit).Init();
                    temp.teamID = GameManager.Instance.currentTeam;
                    if (temp.GetUnitType() is Unit.UnitType.god && GameManager.Instance.GetCurrentTeamObj().isGodRespawned)
                    {
                        AlertManager.AddAlert("Limit 1 god!");
                        return;
                    }
                    _mapManager.AddUnitToMap(_mapManager._selectedTile, temp);
                    break;
                }
                else
                {
                    AlertManager.AddAlert("Building is already occupied.");
                    return;
                }
            case "fix":
                if (_selectedObject is Building)
                {
                    ((Building)_selectedObject).Fix();
                    LoadMapObject(_selectedObject);
                }
                break;
            case "apply_effect_at_target":
                MapManager.Instance.ExecuteOnTarget((target) =>
                {
                    if(target is Unit)
                    {
                        ((Unit)target).applyStatusEffect(StatusEffectDataHolder.Instance.statusEffectData.GetValueOrDefault(parsedCode[1]));
                        processPayment(action);
                    }
                });
                return;
            case "set_applied_effect_at_target":
                MapManager.Instance.ExecuteOnTarget((target) =>
                {
                    if (target is Unit)
                    {
                        ((Unit)target).addAppliedStatusEffect(StatusEffectDataHolder.Instance.statusEffectData.GetValueOrDefault(parsedCode[1]));
                        processPayment(action);
                    }
                });
                return;
            case "precise_shot":
                MapManager.Instance.ExecuteOnTarget((target) =>
                {
                    if (target is Monster)
                    {
                        ((Unit)target).takeDamage(((Unit)_selectedObject).getAttackStrength()*3);
                        processPayment(action);
                    }
                    else if (target is Unit)
                    {
                        ((Unit)target).takeDamage(((Unit)_selectedObject).getAttackStrength());

                        Vector2Int pos = ((Unit)target).position;

                        Healthbar healthbar = MapManager.Instance.GetUnitGraphic(pos).gameObject.GetComponentsInChildren<Healthbar>()[0];
                        healthbar.SetHealth(MapManager.Instance.map.getUnit(pos).getHP());
                        if (MapManager.Instance.map.getUnit(pos).getHP() <= 0)
                        {
                            MapManager.Instance.DeleteUnitGraphic(pos);
                            MapManager.Instance.map.RemoveUnit(pos);
                        }

                        ((Unit)_selectedObject).animator.SetTrigger("Ability");
                        processPayment(action);
                    }
                });
                return;
            case "blessing_of_ice":
                MapManager.Instance.ExecuteOnTarget((target) =>
                {
                    if (target is Unit)
                    {
                        ((Unit)target).applyStatusEffect(StatusEffectDataHolder.Instance.statusEffectData.GetValueOrDefault("frozen"));
                        ((Unit)target).onAttackTimer = 2;

                        ((Unit)_selectedObject).animator.SetTrigger("Ability");
                        processPayment(action);
                    }
                });

                return;
            case "blessing_of_fire":
                MapManager.Instance.ExecuteOnTarget((target) =>
                {
                    if (target is Unit)
                    {
                        ((Unit)target).addAppliedStatusEffect(StatusEffectDataHolder.Instance.statusEffectData.GetValueOrDefault("onFire"));
                        //((Unit)target).onAttackTimer = 2;
                        ((Unit)target).SetUpgradeUnit();

                        ((Unit)_selectedObject).animator.SetTrigger("Ability");
                        processPayment(action);
                    }
                });

                return;
            case "paralysis":
                MapManager.Instance.ExecuteOnTarget((target) =>
                {
                    if (target is Unit)
                    {
                        if (target.teamID != GameManager.Instance.currentTeam)
                        {

                            ((Unit)target).takeDamage(10);

                            Vector2Int pos = ((Unit)target).position;

                            Healthbar healthbar = MapManager.Instance.GetUnitGraphic(pos).gameObject.GetComponentsInChildren<Healthbar>()[0];
                            healthbar.SetHealth(MapManager.Instance.map.getUnit(pos).getHP());
                            if (MapManager.Instance.map.getUnit(pos).getHP() <= 0)
                            {
                                MapManager.Instance.DeleteUnitGraphic(pos);
                                MapManager.Instance.map.RemoveUnit(pos);
                            }
                            else
                            {
                                ((Unit)target).applyStatusEffect(StatusEffectDataHolder.Instance.statusEffectData.GetValueOrDefault("stunned"));
                            }

                            //Projectile projectile = UnitDataHolder.Instance.unitData.GetValueOrDefault("perun").projectile;

                            //MapManager.Instance.SendProjectile(projectile, _selectedObject.position, pos);

                            FXAnimationManager.PlayAnimationAtPosition("Thunder", pos);

                            ((Unit)_selectedObject).animator.SetTrigger("Ability");
                            processPayment(action);
                        }
                    }
                    else if (target is Building)
                    {

                        ((Unit)_selectedObject).animator.SetTrigger("Ability");
                        ((Building)target).DealDamageBuilding(((Building)target).GetBasePounding());
                    }
                });
                    return;
            case "add_effect_to_whole_team":
                foreach (KeyValuePair<Vector2Int, Unit> kvp in MapManager.Instance.map.getAllUnits())
                {
                    if (kvp.Value.teamID == GameManager.Instance.currentTeam)
                        kvp.Value.applyStatusEffect(StatusEffectDataHolder.Instance.statusEffectData.GetValueOrDefault(parsedCode[1]));
                }
                break;
            case "change_building_into":
                Vector2Int pos = _selectedObject.position;
                _mapManager.DeleteBuilding(pos);
                BuildAnimation.Instance.StartAnimation(pos);

                Building building = Instantiate(BuildingDataHolder.GetBuildingData(parsedCode[1]).building);
                building.teamID = GameManager.Instance.GetCurrentTeam();
                building.Init();
                building.position = pos;
                _mapManager.AddBuildingToMap(pos, building);
                HideMenu();
                break;
            case "add_faith_points":
                GameManager.Instance.GetCurrentTeamObj().AddFaithPoints( (int)(int.Parse(parsedCode[1])*EventModifier.Instance.percentBuffFaithPoints));
                GameManager.Instance.RefreshResourceDisplays();
                break;
            default: Debug.Log("Unknown command"); break;
        }
        processPayment(action);
    }

    private void processPayment(ActionData action)
    {
        if (action is PaymentActionData)
        {
            GameManager.Instance.GetCurrentTeamObj().AddFood(-((PaymentActionData)action).foodCost);
            GameManager.Instance.GetCurrentTeamObj().AddGold(-((PaymentActionData)action).goldCost);
            GameManager.Instance.GetCurrentTeamObj().AddWood(-((PaymentActionData)action).woodCost);
            GameManager.Instance.GetCurrentTeamObj().AddFaithPoints(-((PaymentActionData)action).faithCost);
            GameManager.Instance.RefreshResourceDisplays();
        }
    }
}
