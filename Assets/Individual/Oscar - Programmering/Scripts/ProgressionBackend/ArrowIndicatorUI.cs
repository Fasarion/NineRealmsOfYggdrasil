using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowIndicatorUI : MonoBehaviour
{


    private RectTransform rectTransform;

    public RectTransform runtimeParentRectTransform;
    public void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        Debug.Log("Awake called on ChoiceUIManager!");
    }

    // Start is called before the first frame update
    void OnEnable()
    {
       // ProgressionBarContentContainer.onRectBoundsSet += OnRectBoundsSet;
    }

    public void AddSymbolXPosition(float position)
    {
        var parentRect = runtimeParentRectTransform.rect;
        rectTransform.anchoredPosition = new Vector2( runtimeParentRectTransform.anchoredPosition.x - (parentRect.width/2) + position, rectTransform.anchoredPosition.y);
    }
    /*private void OnRectBoundsSet(Rect rect)
    {
        rectTransform.anchoredPosition = new Vector2(rect.width, rectTransform.anchoredPosition.y) ;
    }*/

    // Update is called once per frame
    void Update()
    {
        //SetXPosition();
    }
}
