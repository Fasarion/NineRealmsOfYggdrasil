using System;
using System.Collections;
using System.Collections.Generic;
using Patrik;
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
    
    public Sprite spriteToUpdateTo;

    //public Image image;

    private RectTransform imageTransform;
    //public Button debugButtonToSwapSelectedWeapon;
    
    
    public override void Start()
    {
        base.Start();
        mainWeaponSymbolTransform = GetComponent<RectTransform>();
        mainWeaponSymbolTransform.localScale = new Vector3(baseScale, baseScale, baseScale);
        animatedScale = baseScale;
       
        //imageTransform.sizeDelta = new Vector2(rect.width, rect.height);
    }
    
    

    /*public void OnEnable()
    {
        CombatUIWeaponHandler.onCurrentWeaponUpdated += OnCurrentWeaponUpdated;
    }

    public void OnDisable()
    {
        CombatUIWeaponHandler.onCurrentWeaponUpdated -= OnCurrentWeaponUpdated;
    }*/
    
    
    //For debugging purposes
    public void OnCurrentWeaponUpdatedButton()
    {
        currentlySelectedUltSymbol = spriteToUpdateTo;
        ultImage.sprite = currentlySelectedUltSymbol;
        //var rect = image.sprite.rect;
        ultImage.SetNativeSize();
        //imageTransform.sizeDelta = new Vector2(rect.width, rect.height);
    }
    
    /*public void OnCurrentWeaponUpdated(WeaponType weaponType, WeaponType currentLeftInactiveWeapon, WeaponType currentRightInactiveWeapon)
    {
        //We won't use the left and right inactives here.
        switch (weaponType)
        {
            case WeaponType.Hammer:
            {
                currentlySelectedWeaponSymbol = hammerActiveSymbol;
                break;
            }
            case WeaponType.Sword:
            {
                currentlySelectedWeaponSymbol = swordActiveSymbol;
                break;
            }
            case WeaponType.Mead:
            {
                currentlySelectedWeaponSymbol = meadActiveSymbol;
                break;
            }
            
            case WeaponType.Birds:
            {
                currentlySelectedWeaponSymbol = birdActiveSymbol;
                break;
            }
            case WeaponType.None:
            {
                Debug.Log("The weapon enum was none, which should not be possible!");
                break;
            }
            default:
            {
                Debug.Log("The weapon enum was null which should not be possible!");
                break;
            }
        }
        image.sprite = currentlySelectedWeaponSymbol;
        //var rect = image.sprite.rect;
        image.SetNativeSize();
        //imageTransform.sizeDelta = new Vector2(rect.width, rect.height);
    }*/

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
