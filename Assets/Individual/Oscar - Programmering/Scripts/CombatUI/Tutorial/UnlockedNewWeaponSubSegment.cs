using System.Collections;
using System.Collections.Generic;
using Patrik;
using UnityEngine;

public class UnlockedNewWeaponSubSegment : MonoBehaviour
{
    public NewAbilityUnlockTutorialSegment abilityUnlockTutorialSegment;
    public List<GameObject> weaponUnlockObjects;

    private bool weaponUnlocked;
    public void Start()
    {
        abilityUnlockTutorialSegment = FindObjectOfType<NewAbilityUnlockTutorialSegment>();
        weaponUnlocked = false;
        for (int i = 0; i < weaponUnlockObjects.Count; i++)
        {
            weaponUnlockObjects[i].SetActive(false);
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
            for (int i = 0; i < weaponUnlockObjects.Count; i++)
            {
                weaponUnlockObjects[i].SetActive(false);
            }
          
            abilityUnlockTutorialSegment.SecondWeaponUnlocked(true);
        }
        
    }

    private void OnWeaponUnlocked(int weaponCount)
    {
        if (weaponUnlocked != true)
        {
            weaponUnlocked = true;
            for (int i = 0; i < weaponUnlockObjects.Count; i++)
            {
                weaponUnlockObjects[i].SetActive(true);
            }
        }
       
        
    }
}
