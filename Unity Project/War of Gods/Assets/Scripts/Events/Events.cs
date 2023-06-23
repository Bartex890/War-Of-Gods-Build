using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Events : MonoBehaviour
{
    [SerializeField]
    private List<Event> _events;
    [SerializeField]
    private GameObject _windowEvents;
    [SerializeField]
    private TextMeshProUGUI _name;
    [SerializeField]
    private TextMeshProUGUI _description;
    private Event _lastEvent;
    private bool firstEvent;
    private bool secondEvent;
    public static Events _instance;
    public static Events Instance
    {
        get
        {
            if (_instance == null)
            {
                Debug.LogError("Events is null");
            }
            return _instance;
        }
    }


    private void Awake()
    {
        _instance = this;
        firstEvent = true;
        secondEvent = false;
    }

    private bool GetRangeRounds(Event currentEvent)
    {
        string[] tab = currentEvent.rangeRounds.Split(";");
         if(tab.Contains(GameManager.Instance.currentDay.ToString())) return false;
         else return true;
    }

    private void AddEffect(Event currentEvent)
    {
        int idCode = 0;
        string[] parseCode = currentEvent.effects.Split(";");
        isMoreEvent:
        switch (parseCode[idCode])
        {
            case "resource":
                idCode++;
                switch (parseCode[idCode])
                {
                    case "wood": idCode++; EventModifier.Instance.percentBuffWood = float.Parse(parseCode[idCode]); break;
                    case "gold": idCode++; EventModifier.Instance.percentBuffGold = float.Parse(parseCode[idCode]); break;
                    case "food": idCode++; EventModifier.Instance.percentBuffFood = float.Parse(parseCode[idCode]); break;
                    case "faithPoints": idCode++; EventModifier.Instance.percentBuffFaithPoints = float.Parse(parseCode[idCode]); break;
                    default: Debug.LogWarning("Brakuje co ma zmieniæ!"); break;
                }
                break;
            case "attackMonsters":
                Dictionary<Vector2Int, Unit> dictUnits = MapManager.Instance.map.getAllUnits();
                List<Unit> units = dictUnits.Values.ToList();
                idCode++;
                foreach (Unit unit in units)
                {
                    if(unit is Monster)
                    {
                        unit.multiplyDamage(float.Parse(parseCode[idCode]));
                    }
                }
                break;
            case "range": idCode++; EventModifier.Instance.percentBuffRangeMove = float.Parse(parseCode[idCode]); break;
            case "spawnMonsters": idCode++; MapManager.Instance.SpawnMonsters(int.Parse(parseCode[idCode])); break;
            case "-": Debug.Log("Nic siê nie dzieje"); break;
            case "boost": break;
            case "price": break;
            case "hp": break;
            case "pouding": break;
            default: Debug.LogWarning("Nie obs³uguje tego eventu"); break;
        }
        idCode++;
        if (idCode < parseCode.Length )
        {
            if(parseCode[idCode] != null)
            {
                goto isMoreEvent;
            }
            
        }

    }

    private void RemoveEffect()
    {
        int idCode = 0;
        string[] parseCode = _lastEvent.effects.Split(";");
        isMoreEventRemove:
        switch (parseCode[idCode])
        {
            case "resource":
                idCode++;
                switch (parseCode[idCode])
                {
                    case "wood": idCode++; EventModifier.Instance.percentBuffWood = 1f; break;
                    case "gold": idCode++; EventModifier.Instance.percentBuffGold = 1f; break;
                    case "food": idCode++; EventModifier.Instance.percentBuffFood = 1f; break;
                    case "faithPoints": idCode++; EventModifier.Instance.percentBuffFaithPoints = 1f; break;
                    default: Debug.LogWarning("Brakuje co ma zmieniæ!"); break;
                }
                break;
            case "attackMonsters":
                idCode++;
                Dictionary<Vector2Int, Unit> dictUnits = MapManager.Instance.map.getAllUnits();
                List<Unit> units = dictUnits.Values.ToList();
                float f = 1 / float.Parse(parseCode[idCode]);
                foreach (Unit unit in units)
                {
                    if (unit is Monster)
                    {
                        unit.multiplyDamage(f);
                    }
                }
                break;
            case "range": idCode++; EventModifier.Instance.percentBuffRangeMove = 1f; break;
            case "boost": break;
            case "price": break;
            case "hp": break;
            case "pouding": break;
            default: break;
        }
        idCode++;
        if (idCode < parseCode.Length)
        {
            if (parseCode[idCode] != null)
            {
                goto isMoreEventRemove;
            }
        }
    }

    public void RandomEvent()
    {
        if (_lastEvent != null)
        {
            RemoveEffect();
        }
        if (firstEvent)
        {
            _name.text = "Save Your Kingdom";
            _description.text = "The goal of the game is to kill all the enemies and capture all their bases. Save your kingdom from the invaders!";
            _windowEvents.SetActive(true);
            secondEvent = true;
            firstEvent = false;
        }
        else
        {
            Event selectedEvent = _events[Random.Range(0, _events.Count)];
            while (GetRangeRounds(selectedEvent))
            {
                selectedEvent = _events[Random.Range(0, _events.Count)];
            }
            _name.text = selectedEvent.name;
            _description.text = selectedEvent.description;
            _windowEvents.SetActive(true);
            AddEffect(selectedEvent);
            _lastEvent = selectedEvent;
        }
        
        
    }

    public void SetActiveWindowEvents()
    {
        if (_lastEvent != null)
        {
            _windowEvents.SetActive(true);
        }
        else if (secondEvent)
        {
            _windowEvents.SetActive(true);
            secondEvent = false;
        }
    }
}
