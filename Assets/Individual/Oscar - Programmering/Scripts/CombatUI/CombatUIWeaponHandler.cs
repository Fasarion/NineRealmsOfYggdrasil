using System;
using System.Collections;
using System.Collections.Generic;
using Patrik;
using UnityEngine;

public class CombatUIWeaponHandler : MonoBehaviour
{

    public WeaponType currentWeapon;

    public WeaponType debugWeaponToUpdateTo;
    //WeaponType currentWeaponType;
    public CombatUIMainWeaponSymbol combatUIMainWeaponSymbol;

    public WeaponType currentLeftInactiveWeapon;
    public WeaponType currentRightInactiveWeapon;

    public static Action<WeaponType, WeaponType, WeaponType> onCurrentWeaponUpdated;
    //Placeholder for debugging purposes;
    //private WeaponType hammerWeapon = WeaponType.Hammer;
    //private WeaponType swordWeapon = WeaponType.Sword;
    //private WeaponType birdWeapon = WeaponType.Birds;
    
    //private WeaponType meadWeapon = WeaponType.Mead;
    //I am missing a class to reference to get the current types of weapons from the player
    public List<WeaponType> currentPlayerWeapons;

    // Start is called before the first frame update
    void Start()
    {
        
        //currentPlayerWeapons.Add(hammerWeapon);
        //currentPlayerWeapons.Add(swordWeapon);
        //currentPlayerWeapons.Add(birdWeapon);
    }

    public void DebugButtonPressed()
    {
        UpdateCurrentWeapon(debugWeaponToUpdateTo);
    }
    //We're going to need a list of the players current weapons here.
    public void UpdateCurrentWeapon(WeaponType weaponType)
    {
        //currentInactiveWeapons.Clear();
        //var selectableWeapons = new List<WeaponType>(currentPlayerWeapons);
        

        //Swap inactive and active weapons
        if(currentLeftInactiveWeapon == weaponType)
        {
            currentLeftInactiveWeapon = currentWeapon;
            Debug.Log("Swapped to left weapon");
        }
        else if (currentRightInactiveWeapon == weaponType)
        {
            currentRightInactiveWeapon = currentWeapon;
            Debug.Log("Swapped to right weapon");
        }
        else
        {
            Debug.Log("Something went wrong! You tried to set a weapon that was not one of your inactive weapons!");
        }
        
        currentWeapon = weaponType;
        
        
        //currentInactiveWeapons.AddRange(selectableWeapons);
                
        onCurrentWeaponUpdated?.Invoke(currentWeapon, currentLeftInactiveWeapon, currentRightInactiveWeapon);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
