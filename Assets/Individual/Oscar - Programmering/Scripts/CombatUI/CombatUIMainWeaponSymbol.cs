using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CombatUIMainWeaponSymbol : CombatUIWeaponSymbol
{
    public float baseScale;

    private float animatedScale;
    public float maxAnimatedScale;

    private RectTransform mainWeaponSymbolTransform;
    public float duration = 5.0f;
    private float currentTime = 0;
    //private float startTime;
    // Start is called before the first frame update
    private bool shouldExpand;
    
    private Sprite debugSpriteToUpdateTo;

    //public Image image;

    private RectTransform imageTransform;
    //public Button debugButtonToSwapSelectedWeapon;
    
    
    public override void Start()
    {
        base.Start();
        mainWeaponSymbolTransform = GetComponent<RectTransform>();
        mainWeaponSymbolTransform.localScale = new Vector3(baseScale, baseScale, baseScale);
        animatedScale = baseScale;
    }

    //For debugging purposes
    public void OnCurrentWeaponUpdatedButton()
    {
        //currentlySelectedUltSymbol = spriteToUpdateTo;
        ultImage.sprite = currentlySelectedUltSymbol;
        //var rect = image.sprite.rect;
        ultImage.SetNativeSize();
        //imageTransform.sizeDelta = new Vector2(rect.width, rect.height);
    }
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
