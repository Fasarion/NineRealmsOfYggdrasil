using System.Collections;
using System.Collections.Generic;
using Mono.Cecil.Cil;
using Patrik;
using UnityEngine;

public class CombatUIUltimateProgressBarController : BaseControllerMVC
{
    
    public void OnEnable()
    {
        EventManager.OnEnergyChange += OnEnergyChange;
        CombatUIWeaponHandlerModel.onCurrentWeaponUpdated += OnCurrentWeaponUpdated;
        CombatUIWeaponHandlerModel.onStartingWeaponSet += OnStartingWeaponsSet;

    }

    private void OnStartingWeaponsSet(WeaponType mainWeapon, WeaponType leftWeapon, WeaponType rightWeapon)
    {
        for(int i = 0; i<app.view.combatUIUltimateProgressBarViews.Count; i++) 
        {
            var ultimateProgressBarView = app.view.combatUIUltimateProgressBarViews[i];
            for (int j = 0; j < app.model.combatUIUltimateProgressBarModel.Count; j++)
            {
                var ultimateProgressBarModel = app.model.combatUIUltimateProgressBarModel[j];
                if (ultimateProgressBarModel.identifier == ultimateProgressBarView.identifier)
                {
                    
                    var weaponSymbolType = ultimateProgressBarView.symbolType;
                    ultimateProgressBarModel.currentWeaponType =
                        SetWeapons(weaponSymbolType, mainWeapon, leftWeapon, rightWeapon);
                }
            }
        }
    }

    private void OnCurrentWeaponUpdated(WeaponType mainWeapon, WeaponType leftWeapon, WeaponType rightWeapon)
    {
        for(int i = 0; i<app.view.combatUIUltimateProgressBarViews.Count; i++) 
        {
            var ultimateProgressBarView = app.view.combatUIUltimateProgressBarViews[i];
            for (int j = 0; j < app.model.combatUIUltimateProgressBarModel.Count; j++)
            {
                var ultimateProgressBarModel = app.model.combatUIUltimateProgressBarModel[j];
                if (ultimateProgressBarModel.identifier == ultimateProgressBarView.identifier)
                {
                    var weaponSymbolType = ultimateProgressBarView.symbolType;

                    ultimateProgressBarModel.currentWeaponType =
                        SetWeapons(weaponSymbolType, mainWeapon, leftWeapon, rightWeapon);
                    LoadCurrentEnergy(ultimateProgressBarModel, ultimateProgressBarView);
                }
            }
        }
    }

    private void OnEnergyChange(WeaponType weaponTypeEnergyChanged, float currentEnergy, float maxEnergy)
    {
        for(int i = 0; i<app.view.combatUIUltimateProgressBarViews.Count; i++) 
        {
            var ultimateProgressBarView = app.view.combatUIUltimateProgressBarViews[i];
            for (int j = 0; j < app.model.combatUIUltimateProgressBarModel.Count; j++)
            {
                var ultimateProgressBarModel = app.model.combatUIUltimateProgressBarModel[j];
                if (ultimateProgressBarModel.identifier == ultimateProgressBarView.identifier)
                {
                    if (ultimateProgressBarModel.currentWeaponType == weaponTypeEnergyChanged)
                    {
                        ultimateProgressBarView.slider.maxValue = maxEnergy;
                        ultimateProgressBarView.slider.value = currentEnergy;

                        ultimateProgressBarModel.currentEnergy = currentEnergy;
                        ultimateProgressBarModel.maxEnergy = maxEnergy;

                    }
                }
            }
            
        }
    }

    public void OnDisable()
    {
        EventManager.OnEnergyChange -= OnEnergyChange;
        CombatUIWeaponHandlerModel.onCurrentWeaponUpdated -= OnCurrentWeaponUpdated;
        CombatUIWeaponHandlerModel.onStartingWeaponSet -= OnStartingWeaponsSet;
    }
    
   

    private void LoadCurrentEnergy(CombatUIUltimateProgressBarModel ultimateProgressBarModel, CombatUIUltimateProgressBarView ultimateProgressBarView)
    {
        switch (ultimateProgressBarModel.currentWeaponType)
        {
            case WeaponType.Hammer:
            {
                ultimateProgressBarModel.currentEnergy = ultimateProgressBarModel.energyData.hammerCurrentEnergy;
                ultimateProgressBarModel.maxEnergy =  ultimateProgressBarModel.energyData.hammerMaxEnergy;
                        
                break;
            }
            case WeaponType.Sword:
            {
                ultimateProgressBarModel.currentEnergy =  ultimateProgressBarModel.energyData.swordCurrentEnergy;
                ultimateProgressBarModel.maxEnergy =  ultimateProgressBarModel.energyData.swordMaxEnergy;

                break;
            }
            case WeaponType.Birds:
            {
                ultimateProgressBarModel.currentEnergy =  ultimateProgressBarModel.energyData.birdCurrentEnergy;
                ultimateProgressBarModel.maxEnergy =  ultimateProgressBarModel.energyData.birdMaxEnergy;

                break;
            }
        }
        ultimateProgressBarView.slider.maxValue = ultimateProgressBarModel.maxEnergy;
        ultimateProgressBarView.slider.value = ultimateProgressBarModel.currentEnergy;
    }

    public WeaponType SetWeapons(WeaponSymbolType symbolType, WeaponType mainWeapon, WeaponType leftWeapon, WeaponType rightWeapon)
    {
        WeaponType currentWeaponType = WeaponType.None;
        switch (symbolType)
        {
            case WeaponSymbolType.Main:
            {
                currentWeaponType = mainWeapon;
                break;
            }    
            case WeaponSymbolType.LeftInactive:
            {
                currentWeaponType = leftWeapon;
                break;
            } 
            case WeaponSymbolType.RightInactive:
            {
                currentWeaponType = rightWeapon;
                break;
            }
        }

        return currentWeaponType;
    }
    public override void OnNotification(string p_event_path, Object p_target, params object[] p_data)
   {
       //Not used because all events are external, but must exist as the base class is abstract.
   }   
    /*public override void OnNotification(string p_event_path, Object p_target, params object[] p_data)
   {

       switch (p_event_path)
       {
           case NotificationMVC.UltimateProgressBarWeaponSetup:
           {
               foreach (var ultimateProgressBarView in app.view.combatUIUltimateProgressBarViews)
               {
                   var ultimateProgressBarModel = (CombatUIUltimateProgressBarModel)p_target;
                   if (ultimateProgressBarModel.identifier == ultimateProgressBarView.identifier)
                   {
                       var weaponSymbolType = ultimateProgressBarView.symbolType;
                       WeaponType mainWeapon = (WeaponType)p_data[0];
                       WeaponType leftWeapon = (WeaponType)p_data[1];
                       WeaponType rightWeapon= (WeaponType)p_data[2];
                  
                       ultimateProgressBarModel.currentWeaponType = SetWeapons(weaponSymbolType, mainWeapon, leftWeapon, rightWeapon);
                   }
                  
                   
               }
               
               
               break;
           }
           case NotificationMVC.UltimateProgressBarWeaponUpdated:
           {
               
               
               foreach (var ultimateProgressBarView in app.view.combatUIUltimateProgressBarViews)
               {
                   var ultimateProgressBarModel = (CombatUIUltimateProgressBarModel)p_target;
                   if (ultimateProgressBarModel.identifier == ultimateProgressBarView.identifier)
                   {
                       var weaponSymbolType = ultimateProgressBarView.symbolType;
                       WeaponType mainWeapon = (WeaponType)p_data[0];
                       WeaponType leftWeapon = (WeaponType)p_data[1];
                       WeaponType rightWeapon= (WeaponType)p_data[2];
                  
                       ultimateProgressBarModel.currentWeaponType = SetWeapons(weaponSymbolType, mainWeapon, leftWeapon, rightWeapon);
                       LoadCurrentEnergy(ultimateProgressBarModel, ultimateProgressBarView);
                   }
                  
                   
               }
               
               break;
           }
           case NotificationMVC.UltimateProgressBarEnergyChanged:
           {
               foreach (var ultimateProgressBarView in app.view.combatUIUltimateProgressBarViews)
               {
                   var ultimateProgressBarModel = (CombatUIUltimateProgressBarModel) p_target;
                   WeaponType weaponTypeEnergyChanged = (WeaponType)p_data[0];
                   float currentEnergy = (float)p_data[1];
                   float maxEnergy = (float)p_data[2];
                   if (ultimateProgressBarModel.identifier == ultimateProgressBarView.identifier)
                   {
                       if (ultimateProgressBarModel.currentWeaponType == weaponTypeEnergyChanged)
                       {
                           ultimateProgressBarView.slider.maxValue = maxEnergy;
                           ultimateProgressBarView.slider.value = currentEnergy;

                           ultimateProgressBarModel.currentEnergy = currentEnergy;
                           ultimateProgressBarModel.maxEnergy = maxEnergy;

                       }
                   }
               }

               break;
           }
           
       }
   }*/
}
