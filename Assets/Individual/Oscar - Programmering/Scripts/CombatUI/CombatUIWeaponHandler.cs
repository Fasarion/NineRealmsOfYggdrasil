using System;
using System.Collections;
using System.Collections.Generic;
using Patrik;
using UnityEngine;

public class CombatUIWeaponHandler : MonoBehaviour
{

    public WeaponType currentWeapon;
    //WeaponType currentWeaponType;
    public CombatUIMainWeaponSymbol combatUIMainWeaponSymbol;

    private WeaponType currentLeftInactiveWeapon;
    private WeaponType currentRightInactiveWeapon;

    public static Action<WeaponType, List<WeaponType>> onCurrentWeaponUpdated;
    //Placeholder for debugging purposes;
    private WeaponType hammerWeapon = WeaponType.Hammer;
    private WeaponType swordWeapon = WeaponType.Sword;
    private WeaponType birdWeapon = WeaponType.Birds;

   
    //private WeaponType meadWeapon = WeaponType.Mead;
    //I am missing a class to reference to get the current types of weapons from the player
    public List<WeaponType> currentPlayerWeapons;
    
    // Start is called before the first frame update
    void Start()
    {
        currentPlayerWeapons.Add(hammerWeapon);
        currentPlayerWeapons.Add(swordWeapon);
        currentPlayerWeapons.Add(birdWeapon);
    }

    //We're going to need a list of the players current weapons here.
    public void UpdateCurrentWeapon(WeaponType weaponType)
    {
        //currentInactiveWeapons.Clear();
        //var selectableWeapons = new List<WeaponType>(currentPlayerWeapons);
        

        //Replace swap inactive and active weapons
        if(currentLeftInactiveWeapon == weaponType)
        {
            currentLeftInactiveWeapon = currentWeapon;
        }
        else if (currentRightInactiveWeapon == weaponType)
        {
            currentRightInactiveWeapon = currentWeapon;
        }
        
        //selectableWeapons.Remove(weaponType);
        currentWeapon = weaponType;
        //currentInactiveWeapons.AddRange(selectableWeapons);
                
        //onCurrentWeaponUpdated?.Invoke(weaponType, currentInactiveWeapons);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}