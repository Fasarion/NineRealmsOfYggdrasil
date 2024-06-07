using System;
using System.Collections;
using System.Collections.Generic;
using Patrik;
using UnityEngine;

public class UltimateReadySubSegment : MonoBehaviour
{
    public bool useSwedish;
    public LanguageLocalizerBehaviour localizerBehaviour;
    public GameObject firstTimeUltimateReadyObject;
    public GameObject firstTimeUltimateReadyObjectSWE;
    public NewAbilityUnlockTutorialSegment abilityUnlockTutorialSegment;
    
    private bool ultimateUsedFirstTime;

    private bool hasEnoughEnergy;
    // Start is called before the first frame update
    void Start()
    {
        useSwedish = localizerBehaviour.GetLanguage();
        if (!useSwedish)
        {
            firstTimeUltimateReadyObject.SetActive(false);
        }
        else
        {
            firstTimeUltimateReadyObjectSWE.SetActive(false);
        }
       
        ultimateUsedFirstTime = false;
        hasEnoughEnergy = false;
        abilityUnlockTutorialSegment = FindObjectOfType<NewAbilityUnlockTutorialSegment>();
    }

    public void OnEnable()
    {
        EventManager.OnEnergyChange += OnUltimateReady;
        EventManager.OnUltimatePerform += OnUltimatePerformed;
        EventManager.OnWeaponCountSet += OnWeaponUnlocked;
      
    }

    public void OnDisable()
    {
        EventManager.OnEnergyChange -= OnUltimateReady;
        EventManager.OnUltimatePerform -= OnUltimatePerformed;
        EventManager.OnWeaponCountSet -= OnWeaponUnlocked;
    }

    private void OnWeaponUnlocked(int arg0)
    {
        if (hasEnoughEnergy)
        {
            if (!ultimateUsedFirstTime)
            {
                if (!useSwedish)
                {
                    firstTimeUltimateReadyObject.SetActive(true);
                }
                else
                {
                    firstTimeUltimateReadyObjectSWE.SetActive(true);
                }
            }
        }
    }

    private void OnUltimatePerformed(WeaponType weaponType, AttackData attackData)
    {
        if (!ultimateUsedFirstTime)
        {
            ultimateUsedFirstTime = true;
            if (!useSwedish)
            {
                firstTimeUltimateReadyObject.SetActive(false);
            }
            else
            {
                firstTimeUltimateReadyObjectSWE.SetActive(false);
            }
            
            abilityUnlockTutorialSegment.UltimateReady(true);
        }
    }

    private void OnUltimateReady(WeaponType weaponType, float currentEnergy, float maxEnergy)
    {
        if (currentEnergy >= maxEnergy)
        {
            hasEnoughEnergy = true;
        }

        if (abilityUnlockTutorialSegment.hasUnlockedSecondWeapon)
        {
            if (!ultimateUsedFirstTime)
            {
                
                if (hasEnoughEnergy)
                {
                    if (!useSwedish)
                    {
                        firstTimeUltimateReadyObject.SetActive(true);
                    }
                    else
                    {
                        firstTimeUltimateReadyObjectSWE.SetActive(true);
                    }
                }
            }
        }
       
       
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
