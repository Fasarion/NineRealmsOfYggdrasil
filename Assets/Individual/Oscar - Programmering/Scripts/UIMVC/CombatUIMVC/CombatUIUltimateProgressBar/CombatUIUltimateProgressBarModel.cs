using System;
using System.Collections;
using System.Collections.Generic;
using Patrik;
using UnityEngine;

public class CombatUIUltimateProgressBarModel : ElementMVC
{
    public WeaponType currentWeaponType;

  
    public float currentEnergy;
    public float maxEnergy;
    public CombatUIEnergyData energyData;

    public void Start()
    {
        energyData = FindObjectOfType<CombatUIEnergyData>();
    }

    public void OnEnable()
    {
        EventManager.OnEnergyChange += OnEnergyChange;
        CombatUIWeaponHandlerModel.onCurrentWeaponUpdated += OnCurrentWeaponUpdated;
        CombatUIWeaponHandlerModel.onStartingWeaponSet += OnStartingWeaponsSet;

    }
    
    public void OnDisable()
    {
        EventManager.OnEnergyChange -= OnEnergyChange;
        CombatUIWeaponHandlerModel.onCurrentWeaponUpdated -= OnCurrentWeaponUpdated;
        CombatUIWeaponHandlerModel.onStartingWeaponSet -= OnStartingWeaponsSet;
    }

    private void OnStartingWeaponsSet(WeaponType mainWeapon, WeaponType leftWeapon, WeaponType rightWeapon)
    {
        app.Notify(NotificationMVC.UltimateProgressBarWeaponSetup, this, mainWeapon, leftWeapon, rightWeapon, currentWeaponType);
    }

    private void OnCurrentWeaponUpdated(WeaponType mainWeapon, WeaponType leftWeapon, WeaponType rightWeapon)
    {
        app.Notify(NotificationMVC.UltimateProgressBarWeaponUpdated, this, mainWeapon, leftWeapon, rightWeapon,currentWeaponType);
    }

    private void OnEnergyChange(WeaponType weaponTypeEnergyChanged, float currentEnergy, float maxEnergy)
    {
        app.Notify(NotificationMVC.UltimateProgressBarEnergyChanged, this, weaponTypeEnergyChanged,currentEnergy, maxEnergy);
    }
    
}
