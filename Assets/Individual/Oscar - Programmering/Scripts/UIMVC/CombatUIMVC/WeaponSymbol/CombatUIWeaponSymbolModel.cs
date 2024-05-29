using System.Collections;
using System.Collections.Generic;
using Patrik;
using UnityEngine;

public enum WeaponSymbolType
{
    Main,
    LeftInactive,
    RightInactive
}



public class CombatUIWeaponSymbolModel : ElementMVC
{
    
    public Sprite currentlySelectedUltSymbol;
    public Sprite currentlySelectedNormalSymbol;
    public Sprite currentlySelectedSpecialSymbol;
    public Sprite currentlySelectedPassiveSymbol;
    public void OnEnable()
    {
        CombatUIWeaponHandlerModel.onCurrentWeaponUpdated += OnCurrentWeaponUpdated;
        CombatUIWeaponHandlerModel.onStartingWeaponSet += OnStartingWeaponSetup;
    }
    private void OnStartingWeaponSetup(WeaponSetupData mainWeaponData, WeaponSetupData leftInactiveWeaponData, WeaponSetupData rightInactiveWeaponData)
    {
        app.Notify(NotificationMVC.WeaponSymbolCurrentWeaponUpdated,this,identifier ,mainWeaponData.WeaponType, leftInactiveWeaponData.WeaponType, rightInactiveWeaponData.WeaponType);
    }
    
    private void OnCurrentWeaponUpdated(WeaponSetupData mainWeaponData, WeaponSetupData leftInactiveWeaponData, WeaponSetupData rightInactiveWeaponData)
    {
        app.Notify(NotificationMVC.WeaponSymbolCurrentWeaponUpdated,this,identifier ,mainWeaponData, leftInactiveWeaponData, rightInactiveWeaponData);
    }

    public void OnDisable()
    {
        CombatUIWeaponHandler.onCurrentWeaponUpdated -= OnCurrentWeaponUpdated;
        CombatUIWeaponHandler.onStartingWeaponSet -= OnStartingWeaponSetup;
    }
}
