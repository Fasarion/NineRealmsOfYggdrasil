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
    private void OnStartingWeaponSetup(List<WeaponSetupData> weaponSetupDataList)
    {
        app.Notify(NotificationMVC.WeaponSymbolCurrentWeaponUpdated,this,identifier ,weaponSetupDataList);
    }
    
    private void OnCurrentWeaponUpdated(List<WeaponSetupData> weaponSetupDataList)
    {
        app.Notify(NotificationMVC.WeaponSymbolCurrentWeaponUpdated,this,identifier ,weaponSetupDataList);
    }

    public void OnDisable()
    {
        CombatUIWeaponHandler.onCurrentWeaponUpdated -= OnCurrentWeaponUpdated;
        CombatUIWeaponHandler.onStartingWeaponSet -= OnStartingWeaponSetup;
    }
}
