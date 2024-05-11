using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatUIMainWeaponSymbolModel : ElementMVC
{
    public float animatedScale;
    private float currentTime = 0;
    private bool shouldExpand;
    public RectTransform mainWeaponSymbolTransform;
    private Sprite debugSpriteToUpdateTo;
    public RectTransform imageTransform;

    public float duration;
    public float baseScale;
    public float maxAnimatedScale;
    
    void Update()
    {
        if (currentTime >= duration)
        {
            currentTime = 0;
            shouldExpand = !shouldExpand;

        }
        else
        {
            if (shouldExpand)
            {
                animatedScale = Mathf.SmoothStep(baseScale, maxAnimatedScale, currentTime/duration);
            }
            else
            {
                animatedScale = Mathf.SmoothStep(maxAnimatedScale, baseScale , currentTime/duration);
            }
            
            mainWeaponSymbolTransform.localScale = new Vector3(animatedScale, animatedScale, animatedScale);
            currentTime += Time.deltaTime;
        }
       
        
    }
    
}
