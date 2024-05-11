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
        CombatUIWeaponHandler.onCurrentWeaponUpdated += OnCurrentWeaponUpdated;
        CombatUIWeaponHandler.onStartingWeaponSet += OnCurrentWeaponUpdated;
    }

    private void OnCurrentWeaponUpdated(WeaponType weaponType, WeaponType currentLeftInactiveWeapon, WeaponType currentRightInactiveWeapon)
    {
        app.Notify(NotificationMVC.WeaponSymbolCurrentWeaponUpdated,this, weaponType, currentLeftInactiveWeapon, currentRightInactiveWeapon);
    }

    public void OnDisable()
    {
        CombatUIWeaponHandler.onCurrentWeaponUpdated -= OnCurrentWeaponUpdated;
        CombatUIWeaponHandler.onStartingWeaponSet -= OnCurrentWeaponUpdated;
    }
}
