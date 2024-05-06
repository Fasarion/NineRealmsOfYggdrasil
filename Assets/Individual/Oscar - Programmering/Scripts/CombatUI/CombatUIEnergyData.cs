using System;
using System.Collections;
using System.Collections.Generic;
using Patrik;
using UnityEngine;

public class CombatUIEnergyData : MonoBehaviour
{
    
    public float hammerCurrentEnergy;
    public float swordCurrentEnergy;
    public float birdCurrentEnergy;
    
    public float hammerMaxEnergy;
    public float swordMaxEnergy;
    public float birdMaxEnergy;
    public void OnEnable()
    {
        EventManager.OnEnergyChange += OnEnergyChange;
    }
    
    public void OnDisable()
    {
        EventManager.OnEnergyChange -= OnEnergyChange;
    }

    private void OnEnergyChange(WeaponType weaponType, float currentEnergy, float maxEnergy)
    {
        switch (weaponType)
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
    }
}
