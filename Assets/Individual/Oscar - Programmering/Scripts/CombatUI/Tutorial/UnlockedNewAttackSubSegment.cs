using System;
using System.Collections;
using System.Collections.Generic;
using Patrik;
using UnityEngine;

public class UnlockedNewAttackSubSegment : MonoBehaviour
{

    public NewAbilityUnlockTutorialSegment abilityUnlockTutorialSegment;
    public GameObject rightClickSegmentObjects;

    private bool rightClickUnlocked;
    public void Start()
    {
        abilityUnlockTutorialSegment = FindObjectOfType<NewAbilityUnlockTutorialSegment>();
        rightClickUnlocked = false;
        rightClickSegmentObjects.SetActive(false);
    }

    public void OnEnable()
    {
        EventManager.OnSpecialAttackUnlocked += OnRightClickUnlocked;
        EventManager.OnUpdateAttackAnimation += OnRightClickActivated;
    }

    private void OnRightClickActivated(AttackType attackType, bool canSpecialAttack)
    {
        if (attackType == AttackType.Special && canSpecialAttack)
        {
            if (rightClickUnlocked)
            {
            
                rightClickSegmentObjects.SetActive(false);
                abilityUnlockTutorialSegment.RightClickUnlocked(true);
            }
        }
      
    }

    private void OnRightClickUnlocked(WeaponType weaponType)
    {
        if (!rightClickUnlocked)
        {
            rightClickUnlocked = true;
            rightClickSegmentObjects.SetActive(true);
        }
       
        
    }
    // Start is called before the first frame update
   

    // Update is called once per frame
    void Update()
    {
        
    }
}
