using System.Collections;
using System.Collections.Generic;
using Patrik;
using UnityEngine;

public class CombatUIWeaponHandlerController : BaseControllerMVC
{
    public override void OnNotification(string p_event_path, Object p_target, params object[] p_data)
    {

        var weaponHandlerModel = app.model.weaponHandlerModel;
        var weaponHandlerView = app.view.weaponHandlerView;
        switch (p_event_path)
        {
            case NotificationMVC.WeaponSetupWeaponHandler:
            {
                var weaponBehaviour = (WeaponBehaviour)p_data[0];
                var isActiveWeapon = (bool) p_data[1];
                if (isActiveWeapon)
                {
                    weaponHandlerView.currentWeapon = weaponBehaviour.WeaponType;
                    weaponHandlerModel.currentWeaponSet = true;
                }
                else
                {
                    weaponHandlerView.currentLeftInactiveWeapon = weaponBehaviour.WeaponType;
                    weaponHandlerModel.currentLeftWeaponSet = true;
                }

                if (weaponHandlerModel.currentWeaponSet && weaponHandlerModel.currentLeftWeaponSet)
                {
                    SetStartingWeapons(weaponHandlerView);
                }
                break;
            }
            case NotificationMVC.WeaponSwitchedWeaponHandler:
            {
                var weaponBehaviour = (WeaponBehaviour)p_data[0];
                UpdateCurrentWeapon(weaponBehaviour.WeaponType, weaponHandlerView, weaponHandlerModel);
                break;
            }
        }
    }
    
    
    private void SetStartingWeapons(CombatUIWeaponHandlerView weaponHandlerView)
    {
        CombatUIWeaponHandlerModel.onStartingWeaponSet?.Invoke(weaponHandlerView.currentWeapon, weaponHandlerView.currentLeftInactiveWeapon, weaponHandlerView.currentRightInactiveWeapon);
    }
    
    public void UpdateCurrentWeapon(WeaponType weaponType, CombatUIWeaponHandlerView weaponHandlerView, CombatUIWeaponHandlerModel weaponHandlerModel)
    {
        //currentInactiveWeapons.Clear();
        //var selectableWeapons = new List<WeaponType>(currentPlayerWeapons);
        

        //Swap inactive and active weapons
        if(weaponHandlerView.currentLeftInactiveWeapon == weaponType)
        {
            weaponHandlerView.currentLeftInactiveWeapon = weaponHandlerView.currentWeapon;
            //Debug.Log("Swapped to left weapon");
        }
        else if (weaponHandlerView.currentRightInactiveWeapon == weaponType)
        {
            weaponHandlerView.currentRightInactiveWeapon = weaponHandlerView.currentWeapon;
            //Debug.Log("Swapped to right weapon");
        }
        else
        {
            Debug.Log("Something went wrong! You tried to set a weapon that was not one of your inactive weapons!");
        }
        
        weaponHandlerView.currentWeapon = weaponType;
        
        
        //currentInactiveWeapons.AddRange(selectableWeapons);
                
        CombatUIWeaponHandlerModel.onCurrentWeaponUpdated?.Invoke(   weaponHandlerView.currentWeapon,    weaponHandlerView.currentLeftInactiveWeapon,    weaponHandlerView.currentRightInactiveWeapon);
    }
}
