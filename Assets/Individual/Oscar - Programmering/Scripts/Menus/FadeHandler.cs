using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using UnityEngine.UI;

public class FadeHandler : MonoBehaviour
{ 
    public Image imageToFade;
    private Color currentColor;
    private float currentAlpha;
    
    public void Awake()
    {
        currentColor = imageToFade.color;
    }

    public void OnFade()
    {
        
        imageToFade.color = new Color(currentColor.r,currentColor.g, currentColor.b, currentAlpha);
    }

    public void Update()
    {
        
        
    }

    public bool IsFadeFinished()
    {
        if (currentAlpha >= 1)
        {
            return true;
        }
        else
        {
            return false;
        }
           
    }

    public IEnumerator Fade()
    {
        
        return new WaitUntil(IsFadeFinished);
    }

}
