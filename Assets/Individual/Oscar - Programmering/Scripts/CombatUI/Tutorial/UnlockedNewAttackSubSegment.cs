using System;
using System.Collections;
using System.Collections.Generic;
using Patrik;
using UnityEngine;

public class UnlockedNewAttackSubSegment : MonoBehaviour
{
    public LanguageLocalizerBehaviour localizerBehaviour;
    public NewAbilityUnlockTutorialSegment abilityUnlockTutorialSegment;
    public GameObject rightClickSegmentObjects;
    public GameObject rightClickSegmentObjectsSWE;
    public bool useSwedish;

    private bool rightClickUnlocked;
    public void Start()
    {
        useSwedish = localizerBehaviour.GetLanguage();
        abilityUnlockTutorialSegment = FindObjectOfType<NewAbilityUnlockTutorialSegment>();
        rightClickUnlocked = false;
        if (!useSwedish)
        {
            rightClickSegmentObjects.SetActive(false);
        }
        else
        {
            rightClickSegmentObjectsSWE.SetActive(false);
        }
        
    }

    public void OnEnable()
    {
        EventManager.OnSpecialAttackUnlocked += OnRightClickUnlocked;
        EventManager.OnUpdateAttackAnimation += OnRightClickActivated;
    }
    
    public void OnDisable()
    {
        EventManager.OnSpecialAttackUnlocked -= OnRightClickUnlocked;
        EventManager.OnUpdateAttackAnimation -= OnRightClickActivated;
    }

    private void OnRightClickActivated(AttackType attackType, bool canSpecialAttack)
    {
        if (attackType == AttackType.Special && canSpecialAttack)
        {
            if (rightClickUnlocked)
            {
            
                if (!useSwedish)
                {
                    rightClickSegmentObjects.SetActive(false);
                }
                else
                {
                    rightClickSegmentObjectsSWE.SetActive(false);
                }
                abilityUnlockTutorialSegment.RightClickUnlocked(true);
            }
        }
      
    }

    private void OnRightClickUnlocked(WeaponType weaponType)
    {
        if (!rightClickUnlocked)
        {
            rightClickUnlocked = true;
            
            if (!useSwedish)
            {
                rightClickSegmentObjects.SetActive(true);
            }
            else
            {
                rightClickSegmentObjectsSWE.SetActive(true);
            }
        }
       
        
    }
    // Start is called before the first frame update
   

    // Update is called once per frame
    void Update()
    {
        
    }
}
