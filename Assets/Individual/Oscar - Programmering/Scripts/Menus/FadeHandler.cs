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

    public float fadeDelay;
    private float currentFadeDelay;
    //public AnimationCurve fadeOutCurve;

    public float fadeTimeMultiplier = 1f;
    public void Awake()
    {
        currentColor = imageToFade.color;
    }

    public void OnFadeIn()
    {

        currentFadeDelay = 0;
        fadeTime = 0;
        fadeMode = FadeMode.FadeIn;
       
        StartCoroutine(Fade());
    }
    
    public void OnFadeOut()
    {
        currentFadeDelay = 0;
        fadeTime = 0;
        fadeMode = FadeMode.FadeOut;
        StartCoroutine(Fade());
    }
    
    public IEnumerator Fade()
    {
        while (currentFadeDelay < fadeDelay)
        {
            currentFadeDelay += Time.deltaTime;
            if (fadeMode == FadeMode.FadeIn)
            {
                currentAlpha = fadeInCurve.Evaluate(1 - fadeTime);
            }
            else if(fadeMode == FadeMode.FadeOut)
            {
                currentAlpha = fadeInCurve.Evaluate(fadeTime);
                
            }
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

        //Put event to tell the game that the fade is completed here.
        EventManager.OnScreenFadeComplete?.Invoke();


    }

}
