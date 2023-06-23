using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIHider : MonoBehaviour
{
    [SerializeField]
    private GameObject[] _uiToHide;
    private bool _isHidden = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            _isHidden = !_isHidden;
            foreach (GameObject go in _uiToHide)
            {
                go.SetActive(!_isHidden);
            }
        }
    }
}
