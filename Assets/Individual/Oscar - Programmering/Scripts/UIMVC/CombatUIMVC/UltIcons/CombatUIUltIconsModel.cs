using System;
using System.Collections;
using System.Collections.Generic;
using Patrik;
using UnityEngine;

public class CombatUIUltIconsModel : ElementMVC
{
    
    [NonSerialized]public List<WeaponType> weaponTypes;
   
    public int activeUltCounter;
    [NonSerialized]public int symbolCounter;
    
    private void Awake()
    {
        weaponTypes = new List<WeaponType>();
        symbolCounter = 0;
        activeUltCounter = 0;
    }
    
    void Start()
    {
        weaponTypes.Clear();
    }
    
    public void OnEnable()
    {
        EventManager.OnEnergyChange += OnUltimate;
        EventManager.OnSetupWeapon += OnWeaponSetup;
    }

    public void OnDisable()
    {
        EventManager.OnEnergyChange -= OnUltimate;
        EventManager.OnSetupWeapon -= OnWeaponSetup;
    }

    private void OnUltimate(WeaponType weaponType, float currentEnergy, float maxEnergy)
    {
        app.Notify(NotificationMVC.UltimateUsedUltIcons, this,  weaponType, currentEnergy, maxEnergy);
    }
    private void OnWeaponSetup(WeaponBehaviour weaponBehaviour, bool activeWeapon)
    {
        app.Notify(NotificationMVC.WeaponSetupUltIcons, this,  weaponBehaviour, activeWeapon);
    }

    
}
