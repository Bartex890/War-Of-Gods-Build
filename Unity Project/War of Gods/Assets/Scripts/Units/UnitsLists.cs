using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitsLists : MonoBehaviour
{
    private Unit _unit;
    [SerializeField]
    private GameObject _template;
    [SerializeField]
    private Image _sprite;
    [SerializeField]
    private GameObject _buttonSeparate;
    private List<GameObject> listUnits = new List<GameObject>();
    private List<Color> colorListUnits = new List<Color>();
    private List<GameObject> selectedListUnits = new List<GameObject>();
    private MapManager _mapManager;
    private bool _isIncompleteHealth;
    private int lastUnitHP;

    private void Start()
    {
        _mapManager = MapManager.Instance;
        _isIncompleteHealth = false;
    }
    public void SetUnit(Unit unit)
    {
        _unit = unit;
    }
    public void RefreshUnitsList()
    {
        ClearList();
        int numberOfUnits = _unit.numberOfUnits;
        lastUnitHP = _unit.getHP();
        int x = 1;
        int y = 0;
        float itemSlotCellSize = 46f;
        Color color = Color.green;
        _template.gameObject.SetActive(true);
        _sprite.sprite = _unit.GetSprite();
        _template.GetComponentInChildren<Image>().color = color;
        colorListUnits.Add(color);
        Vector2 lastposition = _template.transform.position;
        for (int i = 1; i < numberOfUnits; i++)
        {

            Vector2 anchoredPosition = new Vector2(x * itemSlotCellSize, y * itemSlotCellSize);
            Vector2 newPosition = lastposition + anchoredPosition;
            GameObject temp = Instantiate(_template);
            temp.transform.SetParent(_template.transform.parent);
            temp.transform.position = newPosition;
            if ((i + 1) == numberOfUnits && lastUnitHP < _unit.getBaseHP())
            {
                color = Color.red;
            }
            colorListUnits.Add(color);
            temp.GetComponentInChildren<Image>().color = color;
            temp.GetComponent<IdButton>().id = i;
            listUnits.Add(temp);
            x++;
            if (x > 16)
            {
                x = 0;
                y--;
            }

        }

    }
    public void ClearList()
    {
        if (listUnits.Count != 0)
        {
            for (int i = listUnits.Count - 1; i >= 0; i--)
            {
                Destroy(listUnits[i]);
                listUnits.RemoveAt(i);

            }
        }
        if (selectedListUnits.Count != 0)
        {
            for (int i = selectedListUnits.Count - 1; i >= 0; i--)
            {
                selectedListUnits.RemoveAt(i);

            }
        }
        if (colorListUnits.Count != 0)
        {
            for (int i = colorListUnits.Count - 1; i >= 0; i--)
            {
                colorListUnits.RemoveAt(i);

            }
        }

        _template.gameObject.SetActive(false);
        _buttonSeparate.gameObject.SetActive(false);
    }

    public void SelectUnits()
    {
        //GameObject childObject = gameObject.transform.GetChild(0).gameObject.GetComponent<IdButton>().gameObject;
        GameObject clickedObject = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject;
        
        if (listUnits.Count!=0)
        {
            if (!selectedListUnits.Contains(clickedObject))
            {
                clickedObject.GetComponentInChildren<Image>().color = Color.black;
                if (clickedObject == listUnits[listUnits.Count - 1] && lastUnitHP < _unit.getBaseHP())
                {
                    _isIncompleteHealth = true;
                }
                selectedListUnits.Add(clickedObject);
            }
            else
            {
                clickedObject.GetComponentInChildren<Image>().color = colorListUnits[clickedObject.GetComponent<IdButton>().id];
                if (clickedObject == listUnits[listUnits.Count - 1] && lastUnitHP < _unit.getBaseHP())
                {
                    _isIncompleteHealth = false;
                }
                selectedListUnits.Remove(clickedObject);
                if (selectedListUnits.Count == 0)
                {
                    _buttonSeparate.gameObject.SetActive(false);
                }
            }
        }
        List<Vector2Int> positions = _mapManager.GetListPositions();
        if (selectedListUnits.Count >= 1 && selectedListUnits.Count <listUnits.Count+1 && _mapManager.map.getUnit(positions[0]).moveRange>0 && selectedListUnits.Count <30)
        {
            _buttonSeparate.gameObject.SetActive(true);
            //jak kliknie to gracz wybiera pozycje obok gdzie jednostki odchodz¹
        }
        else
        {
            _buttonSeparate.gameObject.SetActive(false);
        }


    }
    

    public void SeparateSelectedUnits()
    {
        Debug.Log("FUNKCJA separacji");
        _mapManager.isSeparate=true;
        List<Vector2Int> positions = _mapManager.GetListPositions();
        _mapManager._getUnitSeparateMovementOptions(positions[0],1, positions[0], true);
    }

    public void SetNewUnit(Unit unit)
    {
        if (_isIncompleteHealth)
        {
            unit.SetHP(lastUnitHP);
            _unit.SetHP(_unit.getBaseHP());
            _isIncompleteHealth = false;
        }
        unit.numberOfUnits += selectedListUnits.Count;
        _unit.numberOfUnits -= selectedListUnits.Count;
        unit.moveRange = 0;
        ClearList();


    }
    public Unit GetUnit()
    {
        return _unit;
    }
}

