using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgressionBarBehaviour : MonoBehaviour
{
    public float symbolXpos; 
    public enum ProgressionBarCardType
    {
        relic,
        weapon,
        room

    }

    public ProgressionBarCardType CardType;

    public void OnEnable()
    {
        //ProgressionBarContentContainer.onRectBoundsSet += OnRectBoundsSet;
    }

    /*private void OnRectBoundsSet(Rect rect)
    {
        symbolXpos = GetComponent<RectTransform>().anchoredPosition.x;
    }*/
    
    public void SetXPosition()
    {
        symbolXpos = GetComponent<RectTransform>().anchoredPosition.x;
    }
}
