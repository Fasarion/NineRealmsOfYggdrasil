using System.Collections;
using System.Collections.Generic;
using Patrik;
using UnityEngine;

public class UltimateReadySubSegment : MonoBehaviour
{
    public GameObject firstTimeUltimateReadyObject;
    public NewAbilityUnlockTutorialSegment abilityUnlockTutorialSegment;
    
    private bool ultimateUsedFirstTime;

    private bool hasEnoughEnergy;
    // Start is called before the first frame update
    void Start()
    {
        firstTimeUltimateReadyObject.SetActive(false);
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

    private void OnWeaponUnlocked(int arg0)
    {
        if (hasEnoughEnergy)
        {
            if (!ultimateUsedFirstTime)
            {
                firstTimeUltimateReadyObject.SetActive(true);
            }
            

          
        }
    }

    private void OnUltimatePerformed(WeaponType weaponType, AttackData attackData)
    {
        if (!ultimateUsedFirstTime)
        {
            ultimateUsedFirstTime = true;
            firstTimeUltimateReadyObject.SetActive(false);
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
                    firstTimeUltimateReadyObject.SetActive(true);
                }
            }
        }
       
       
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
