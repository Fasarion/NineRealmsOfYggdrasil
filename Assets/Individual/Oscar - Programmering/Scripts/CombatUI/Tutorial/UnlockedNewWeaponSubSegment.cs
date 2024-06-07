using System.Collections;
using System.Collections.Generic;
using Patrik;
using UnityEngine;

public class UnlockedNewWeaponSubSegment : MonoBehaviour
{
    public bool useSwedish;
    public LanguageLocalizerBehaviour localizerBehaviour;
    public NewAbilityUnlockTutorialSegment abilityUnlockTutorialSegment;
    public List<GameObject> weaponUnlockObjects;
    public List<GameObject> weaponUnlockObjectsSWE;

    private bool weaponUnlocked;
    public void Start()
    {
        useSwedish = localizerBehaviour.GetLanguage();
        abilityUnlockTutorialSegment = FindObjectOfType<NewAbilityUnlockTutorialSegment>();
        weaponUnlocked = false;
        if (!useSwedish)
        {
            for (int i = 0; i < weaponUnlockObjects.Count; i++)
            {
                weaponUnlockObjects[i].SetActive(false);
            }
        }
        else
        {
            for (int i = 0; i < weaponUnlockObjectsSWE.Count; i++)
            {
                weaponUnlockObjectsSWE[i].SetActive(false);
            }
        }
        
    }

    public void OnEnable()
    {
        EventManager.OnWeaponCountSet += OnWeaponUnlocked;
        EventManager.OnWeaponSwitch += OnWeaponSwitchActivated;
    }
    
    public void OnDisable()
    {
        EventManager.OnWeaponCountSet -= OnWeaponUnlocked;
        EventManager.OnWeaponSwitch -= OnWeaponSwitchActivated;
    }

    private void OnWeaponSwitchActivated(WeaponSetupData currentWeapon, List<WeaponSetupData> weaponSetupDataList)
    {
        if (weaponUnlocked)
        {
           
            if (!useSwedish)
            {
                for (int i = 0; i < weaponUnlockObjects.Count; i++)
                {
                    weaponUnlockObjects[i].SetActive(false);
                }
            }
            else
            {
                for (int i = 0; i < weaponUnlockObjectsSWE.Count; i++)
                {
                    weaponUnlockObjectsSWE[i].SetActive(false);
                }
            }
           
          
            abilityUnlockTutorialSegment.SecondWeaponUnlocked(true);
        }
        
    }

    private void OnWeaponUnlocked(int weaponCount)
    {
        if (weaponUnlocked != true)
        {
            weaponUnlocked = true;
            if (!useSwedish)
            {
                for (int i = 0; i < weaponUnlockObjects.Count; i++)
                {
                    weaponUnlockObjects[i].SetActive(true);
                }
            }
            else
            {
                for (int i = 0; i < weaponUnlockObjectsSWE.Count; i++)
                {
                    weaponUnlockObjectsSWE[i].SetActive(true);
                }
            }
            
        }
       
        
    }
}
