using System;
using System.Collections;
using System.Collections.Generic;
using Patrik;
using UnityEngine;
using UnityEngine.UI;

public class CombatUIMainWeaponSymbol : MonoBehaviour
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

    public Sprite hammerActiveSymbol;
    public Sprite swordActiveSymbol;
    public Sprite meadActiveSymbol;
    public Sprite birdActiveSymbol;

    private Sprite currentlySelectedWeapon;
    public Sprite spriteToUpdateTo;

    public Image image;

    private RectTransform imageTransform;
    //public Button debugButtonToSwapSelectedWeapon;
    
    
    void Start()
    {
        mainWeaponSymbolTransform = GetComponent<RectTransform>();
        mainWeaponSymbolTransform.localScale = new Vector3(baseScale, baseScale, baseScale);
        animatedScale = baseScale;
        currentlySelectedWeapon = hammerActiveSymbol;
        image.sprite = currentlySelectedWeapon;
        var rect = image.sprite.rect;
        imageTransform = image.GetComponent<RectTransform>();
        image.SetNativeSize();
        //imageTransform.sizeDelta = new Vector2(rect.width, rect.height);
    }

    public void OnEnable()
    {
        CombatUIWeaponHandler.onCurrentWeaponUpdated += OnCurrentWeaponUpdated;
    }

    public void OnDisable()
    {
        CombatUIWeaponHandler.onCurrentWeaponUpdated -= OnCurrentWeaponUpdated;
    }

    //For debugging purposes
    public void OnCurrentWeaponUpdatedButton()
    {
        currentlySelectedWeapon = spriteToUpdateTo;
        image.sprite = currentlySelectedWeapon;
        //var rect = image.sprite.rect;
        image.SetNativeSize();
        //imageTransform.sizeDelta = new Vector2(rect.width, rect.height);
    }
    
    public void OnCurrentWeaponUpdated(WeaponType weaponType, List<WeaponType> inactiveWeapons)
    {
        currentlySelectedWeapon = spriteToUpdateTo;
        image.sprite = currentlySelectedWeapon;
        //var rect = image.sprite.rect;
        image.SetNativeSize();
        //imageTransform.sizeDelta = new Vector2(rect.width, rect.height);
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
