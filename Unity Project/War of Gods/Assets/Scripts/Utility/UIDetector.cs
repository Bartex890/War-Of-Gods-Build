using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIDetector : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public static bool BlockedByUI;

    private void OnLevelWasLoaded(int level)
    {
        BlockedByUI = false;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        BlockedByUI = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        BlockedByUI = false;
    }
}