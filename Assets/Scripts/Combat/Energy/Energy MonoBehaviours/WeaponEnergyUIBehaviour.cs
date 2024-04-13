using System;
using System.Collections;
using System.Collections.Generic;
using Patrik;
using UnityEngine;
using UnityEngine.UI;

public class WeaponEnergyUIBehaviour : MonoBehaviour
{
    [Header("Bar")]
    [SerializeField] private Slider slider;
    
    [Header("Background")]
    [SerializeField] private Image backgroundImage;
    [SerializeField] private Color normalBackgroundColor;
    [SerializeField] private Color fullEnergyBackgroundColor;
    
    [Header("Border")] 
    [SerializeField] private GameObject border;
    
    [Header("Weapon")]
    [SerializeField] private Image weaponImage;

    [Header("Animation")] 
    [SerializeField] private float fullAnimationScaleFactor = 1.3f;
    [SerializeField] private float animationLoopTime = 1f;

    public WeaponBehaviour Weapon {get; private set; }

    private bool isFull => slider.value >= 1;
    private bool isAnimating = false;

    public void Setup(WeaponBehaviour weapon)
    {
        Weapon = weapon;
        weaponImage.sprite = weapon.Sprite;
        
        UpdateBar(0,1);
    }

    public void UpdateBar(float current, float max)
    {
        slider.value = current / max;

        if (isFull)
        {
            backgroundImage.color = fullEnergyBackgroundColor;

            if (!isAnimating)
            {
                StartCoroutine(AnimateFullBarRoutine());
            }
        }
        else
        {
            backgroundImage.color = normalBackgroundColor;
        }
    }

    IEnumerator AnimateFullBarRoutine()
    {
        isAnimating = true;

        float t;
        while (isFull)
        {
            t = 0;
            while (t < 1 && isFull)
            {
                t += Time.deltaTime / animationLoopTime;
                float scale = Mathf.Lerp(1, fullAnimationScaleFactor, t);
                transform.localScale = Vector3.one * scale;
                yield return new WaitForEndOfFrame();
            }
            
            t = 0;
            while (t < 1 && isFull)
            {
                t += Time.deltaTime / animationLoopTime;
                float scale = Mathf.Lerp( fullAnimationScaleFactor, 1, t);
                transform.localScale = Vector3.one * scale;
                yield return new WaitForEndOfFrame();
            }
        }

        transform.localScale = Vector3.one;
        isAnimating = false;
    }

    public void SetActiveWeapon(bool active)
    {
        border.SetActive(active);
    }
}
