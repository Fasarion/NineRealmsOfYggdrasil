using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using UnityEngine.UI;

public enum FadeMode
{
    FadeIn,
    FadeOut
}
public class FadeHandler : MonoBehaviour
{

    private FadeMode fadeMode;
    public Image imageToFade;
    private Color currentColor;
    private float currentAlpha;

    private bool isFading;
    
    private float fadeTime;
    public AnimationCurve fadeInCurve;
    public AnimationCurve fadeOutCurve;

    public float fadeTimeMultiplier = 1f;
    public void Awake()
    {
        currentColor = imageToFade.color;
    }

    public void OnFadeIn()
    {
        //isFading = true;
        fadeTime = 0;
        fadeMode = FadeMode.FadeIn;
        StartCoroutine(Fade());
    }
    
    public void OnFadeOut()
    {
        //isFading = true;
        fadeTime = 0;
        fadeMode = FadeMode.FadeOut;
        StartCoroutine(Fade());
    }

    public void Start()
    {
        
    }

    public void Update()
    {
        /*if (isFading)
        {
            
            fadeTime += Time.deltaTime * fadeTimeMultiplier;
            if (fadeTime >= 1)
            {
                isFading = false;
                return;
            }
            
            if (fadeMode == FadeMode.FadeIn)
            {

                currentAlpha = fadeInCurve.Evaluate(1 - fadeTime);
                    //currentAlpha = Mathf.Lerp(1, 0, fadeTime);
                //currentAlpha = fadeTime * -1;
            }
            else if(fadeMode == FadeMode.FadeOut)
            {
                currentAlpha = fadeInCurve.Evaluate(fadeTime);
                //currentAlpha = fadeOutCurve.Evaluate(fadeTime);
                //currentAlpha = Mathf.Lerp(0, 1, fadeTime);
                //currentAlpha = fadeTime;
            }

            
            imageToFade.color = new Color(currentColor.r,currentColor.g, currentColor.b, currentAlpha);
            
        }*/

       

    }

    public bool IsFadeFinished()
    {
        if (fadeTime >= 1)
        {
            return true;
        }
        return false;
        
           
    }

    public IEnumerator Fade()
    {
        
        while(fadeTime < 1)
        {
            if (fadeMode == FadeMode.FadeIn)
            {

                currentAlpha = fadeInCurve.Evaluate(1 - fadeTime);
                //currentAlpha = Mathf.Lerp(1, 0, fadeTime);
                //currentAlpha = fadeTime * -1;
            }
            else if(fadeMode == FadeMode.FadeOut)
            {
                currentAlpha = fadeInCurve.Evaluate(fadeTime);
                //currentAlpha = fadeOutCurve.Evaluate(fadeTime);
                //currentAlpha = Mathf.Lerp(0, 1, fadeTime);
                //currentAlpha = fadeTime;
            }

            fadeTime += Time.deltaTime * fadeTimeMultiplier;
            imageToFade.color = new Color(currentColor.r,currentColor.g, currentColor.b, currentAlpha);
            yield return null;
        }
        Debug.Log("FadeComplete");
            
       
        
        
    }

}
