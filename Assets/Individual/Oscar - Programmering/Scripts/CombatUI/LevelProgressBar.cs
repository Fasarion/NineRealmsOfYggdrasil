using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelProgressBar : MonoBehaviour
{

    public Slider slider;

    //[Range(0,1)]
    //public float currentValue;

    public TMP_Text levelText;
    
    // Start is called before the first frame update
    void Start()
    {
        slider.value = 0.5f;
        
    }

    public void OnEnable()
    {
        EventManager.OnPlayerExperienceChange += OnPlayerExperienceChange;
    }
    
    public void OnDisable()
    {
        EventManager.OnPlayerExperienceChange -= OnPlayerExperienceChange;
    }
    


    public void OnPlayerExperienceChange(ExperienceInfo experienceInfo)
    {
        slider.value =  experienceInfo.currentXP;
        slider.maxValue = experienceInfo.experienceNeededToLevelUp;
        levelText.text = experienceInfo.currentLevel.ToString();
    }
}
