using System;
using System.Collections;
using System.Collections.Generic;
using Patrik;
using UnityEngine;

public class CombatUIWeaponHandlerModel : ElementMVC
{
    public static Action<WeaponSetupData, WeaponSetupData, WeaponSetupData> onCurrentWeaponUpdated;
    public static Action<WeaponSetupData, WeaponSetupData, WeaponSetupData> onStartingWeaponSet;
    
    [NonSerialized]public bool currentWeaponSet;
    [NonSerialized]public bool currentLeftWeaponSet;

    public void Awake()
    {
        currentWeaponSet = false;
        currentLeftWeaponSet = false;
    }
    
    public void OnEnable()
    {
        EventManager.OnWeaponSwitch += OnWeaponSwitched;
        EventManager.OnSetupWeapon += OnSetupWeapon;
    }
    
    public void OnDisable()
    {
        EventManager.OnWeaponSwitch -= OnWeaponSwitched;
        EventManager.OnSetupWeapon -= OnSetupWeapon;
    }

    private void OnWeaponSwitched(WeaponBehaviour weaponBehaviour)
    {
        app.Notify(NotificationMVC.WeaponSwitchedWeaponHandler, this, weaponBehaviour);
    }

    private void OnSetupWeapon(WeaponSetupData data)
    {
        app.Notify(NotificationMVC.WeaponSetupWeaponHandler, this, data.WeaponBehaviour, data.Active);
    }

   
}
