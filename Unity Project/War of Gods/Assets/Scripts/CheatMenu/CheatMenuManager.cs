using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.AssemblyQualifiedNameParser;
using UnityEngine;

public class CheatMenuManager : MonoBehaviour
{
    private GameObject _cheatMenu;

    private MapManager _mapManager;

    // Start is called before the first frame update
    void Start()
    {
        _mapManager = MapManager.Instance;
        _cheatMenu = transform.Find("CheatMenu").gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.BackQuote))
        {
            _cheatMenu.SetActive(!_cheatMenu.activeInHierarchy);
        }
    }

    public void ParseCode(string code)
    {
        Debug.Log(code);
        string[] parsedCode = code.Split(":");

        switch (parsedCode[0])
        {
            case "spawn":
                Unit temp = Instantiate(UnitDataHolder.Instance.unitData.GetValueOrDefault(parsedCode[1]).unit).Init();
                temp.teamID = GameManager.Instance.currentTeam;
                _mapManager.AddUnitToMap(_mapManager._selectedTile, temp);
                break;
        }
    }
}
