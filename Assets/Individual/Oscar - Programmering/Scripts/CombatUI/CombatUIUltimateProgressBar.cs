using System.Collections;
using System.Collections.Generic;
using Patrik;
using UnityEngine;
using UnityEngine.UI;

public class CombatUIUltimateProgressBar : MonoBehaviour
{

    public Slider slider;

    [SerializeField]private WeaponSymbolType symbolType;
    private WeaponType currentWeaponType;

  
    private float currentEnergy;
    private float maxEnergy;

    [SerializeField]private CombatUIEnergyData energyData;

    public void OnEnable()
    {
        EventManager.OnEnergyChange += OnEnergyChange;
        CombatUIWeaponHandler.onCurrentWeaponUpdated += OnCurrentWeaponUpdated;
        CombatUIWeaponHandler.onStartingWeaponSet += OnStartingWeaponsSet;

    }

    private void OnStartingWeaponsSet(WeaponSetupData mainWeapon, WeaponSetupData leftWeapon, WeaponSetupData rightWeapon)
    {
        switch (symbolType)
        {
            case WeaponSymbolType.Main:
            {
                currentWeaponType = mainWeapon.WeaponType;
                break;
            }    
            case WeaponSymbolType.LeftInactive:
            {
                currentWeaponType = leftWeapon.WeaponType;
                break;
            } 
            case WeaponSymbolType.RightInactive:
            {
                currentWeaponType = rightWeapon.WeaponType;
                break;
            } 
        }
    }


    public void OnDisable()
    {
        EventManager.OnEnergyChange -= OnEnergyChange;
        CombatUIWeaponHandler.onCurrentWeaponUpdated -= OnCurrentWeaponUpdated;
        CombatUIWeaponHandler.onStartingWeaponSet -= OnStartingWeaponsSet;
    }
    
    private void OnCurrentWeaponUpdated(WeaponSetupData mainWeaponData, WeaponSetupData leftWeaponData, WeaponSetupData rightWeaponData)
    {
        //SavePreviousEnergy();
        switch (symbolType)
        {
            
            case WeaponSymbolType.Main:
            {
                
                currentWeaponType = mainWeaponData.WeaponType;
                break;
            }    
            case WeaponSymbolType.LeftInactive:
            {
                currentWeaponType = leftWeaponData.WeaponType;
                break;
            } 
            case WeaponSymbolType.RightInactive:
            {
                currentWeaponType = rightWeaponData.WeaponType;
                break;
            } 
        }
        LoadCurrentEnergy();
    }

    /*private void SavePreviousEnergy()
    {
        switch (currentWeaponType)
        {
            case WeaponType.Hammer:
            {
                hammerCurrentEnergy = currentEnergy;
                hammerMaxEnergy = maxEnergy;
                        
                break;
            }
            case WeaponType.Sword:
            {
                swordCurrentEnergy = currentEnergy;
                swordMaxEnergy = maxEnergy;

                break;
            }
            case WeaponType.Birds:
            {
                birdCurrentEnergy = currentEnergy;
                birdMaxEnergy = maxEnergy;

                break;
            }
        }
    }*/
    
    private void LoadCurrentEnergy()
    {
        switch (currentWeaponType)
        {
            case WeaponType.Hammer:
            {
                currentEnergy = energyData.hammerCurrentEnergy;
                maxEnergy =  energyData.hammerMaxEnergy;
                        
                break;
            }
            case WeaponType.Sword:
            {
                currentEnergy =  energyData.swordCurrentEnergy;
                maxEnergy =  energyData.swordMaxEnergy;

                break;
            }
            case WeaponType.Birds:
            {
                currentEnergy =  energyData.birdCurrentEnergy;
                maxEnergy =  energyData.birdMaxEnergy;

                break;
            }
        }
        slider.maxValue = maxEnergy;
        slider.value = currentEnergy;
       
    }
    
    

    private void OnEnergyChange(WeaponType weaponTypeEnergyChanged, float currentEnergy, float maxEnergy)
    {
        if(currentWeaponType == weaponTypeEnergyChanged)
        {
            slider.maxValue = maxEnergy;
            slider.value =  currentEnergy;
            
            this.currentEnergy = currentEnergy;
            this.maxEnergy = maxEnergy;
            
        }
    }
}
