using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatUIMainWeaponSymbol : MonoBehaviour
{
    public float baseScale;

    public float animatedScale;
    public float maxAnimatedScale;

    public RectTransform mainWeaponSymbolTransform;
    public float duration = 5.0f;

    public float currentTime = 0;
    //private float startTime;
    // Start is called before the first frame update
    public bool shouldExpand;
    void Start()
    {
        mainWeaponSymbolTransform.localScale = new Vector3(baseScale, baseScale, baseScale);
        animatedScale = baseScale;
    }

    // Update is called once per frame
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
                animatedScale = Mathf.SmoothStep(baseScale, maxAnimatedScale, currentTime);
            }
            else
            {
                animatedScale = Mathf.SmoothStep(maxAnimatedScale, baseScale , currentTime);
            }
            
            mainWeaponSymbolTransform.localScale = new Vector3(animatedScale, animatedScale, animatedScale);
            currentTime += Time.deltaTime;
        }
       
        
    }
}
