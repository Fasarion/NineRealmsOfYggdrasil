using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowIndicatorUI : MonoBehaviour
{


    public RectTransform rectTransform;

    public void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    // Start is called before the first frame update
    void OnEnable()
    {
       // ProgressionBarContentContainer.onRectBoundsSet += OnRectBoundsSet;
    }

    public void SetXPosition(float position)
    {
        rectTransform.anchoredPosition = new Vector2(position, rectTransform.anchoredPosition.y);
    }
    /*private void OnRectBoundsSet(Rect rect)
    {
        rectTransform.anchoredPosition = new Vector2(rect.width, rectTransform.anchoredPosition.y) ;
    }*/

    // Update is called once per frame
    void Update()
    {
        
    }
}
