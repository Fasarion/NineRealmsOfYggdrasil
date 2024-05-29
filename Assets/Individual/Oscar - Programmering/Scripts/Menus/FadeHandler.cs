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
    private float fadeTime;
    public AnimationCurve fadeInCurve;

    public float fadeStartDelay;
    private float currentFadeStartDelay;
    
    public float fadeExitDelay;
    private float currentFadeExitDelay;
    //public AnimationCurve fadeOutCurve;

    public float fadeTimeMultiplier = 1f;
    public void Awake()
    {
        currentColor = imageToFade.color;
    }

    public void OnFadeIn()
    {

        currentFadeStartDelay = 0;
        currentFadeExitDelay = 0;
        fadeTime = 0;
        fadeMode = FadeMode.FadeIn;
        
       
        StartCoroutine(Fade());
    }
    
    public void OnFadeOut()
    {
        currentFadeStartDelay = 0;
        currentFadeExitDelay = 0;
        fadeTime = 0;
        fadeMode = FadeMode.FadeOut;
        StartCoroutine(Fade());
    }
    
    public IEnumerator Fade()
    {
        while (currentFadeStartDelay < fadeStartDelay)
        {
            
            if (fadeMode == FadeMode.FadeIn)
            {
                currentAlpha = fadeInCurve.Evaluate(1 - fadeTime);
            }
            else if(fadeMode == FadeMode.FadeOut)
            {
                currentAlpha = fadeInCurve.Evaluate(fadeTime);
                
            }
            currentFadeStartDelay += Time.deltaTime;
            imageToFade.color = new Color(currentColor.r,currentColor.g, currentColor.b, currentAlpha);
            yield return null;
        }
        while(fadeTime < 1)
        {
            if (fadeMode == FadeMode.FadeIn)
            {
                currentAlpha = fadeInCurve.Evaluate(1 - fadeTime);
            }
            else if(fadeMode == FadeMode.FadeOut)
            {
                currentAlpha = fadeInCurve.Evaluate(fadeTime);
                
            }

            fadeTime += Time.deltaTime * fadeTimeMultiplier;
            imageToFade.color = new Color(currentColor.r,currentColor.g, currentColor.b, currentAlpha);
            yield return null;
        }

        while (currentFadeExitDelay < fadeExitDelay)
        {
            currentFadeExitDelay += Time.deltaTime;
            yield return null;
        }

        //Put event to tell the game that the fade is completed here.
        EventManager.OnScreenFadeComplete?.Invoke();

        Debug.Log("Fade Complete");

    }

}
