using System.Collections;
using System.Collections.Generic;
using Patrik;
using UnityEngine;

public class CombatUIWeaponHandlerView : ElementMVC
{
    
    public WeaponType currentWeapon;
    public WeaponType currentLeftInactiveWeapon;
    public WeaponType currentRightInactiveWeapon;
    public WeaponType debugWeaponToUpdateTo;
    
    public void WeaponHandlerDebugButtonPressed()
    {
        app.Notify(NotificationMVC.WeaponSwitchedWeaponHandler, this, debugWeaponToUpdateTo);
    }
}
