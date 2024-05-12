using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NotificationMVC : MonoBehaviour
{

    public const string SetFlashingBoxColor = "flashingBox.setColor";
    
    public const string SetHealthModel = "healthBar.setHealth";
    public const string HealthBarLevelsSetView = "healthBar.healthBarLevelsSet";
    
    public const string ExperienceInfoChanged = "levelProgressBar.experienceInfoChanged";

    public const string UltimateUsedUltIcons = "ultIconds.ultimateUsed";
    public const string WeaponSetupUltIcons= "ultIcons.weaponSetup";
    public const string WeaponSetupWeaponHandler = "weaponHandler.weaponSetup";
    public const string WeaponSwitchedWeaponHandler = "weaponHandler.weaponSwitched";
    
    public const string WeaponSymbolCurrentWeaponUpdated = "weaponSymbol.weaponUpdated";
    //public const string WeaponSymbolStartingWeaponSet = "weaponSymbol.startingWeaponSet";

    public const string MainWeaponSetup = "mainWeaponSymbol.setup";


    public const string UltimateProgressBarWeaponSetup = "ultimateProgressBar.weaponSetup";
    public const string UltimateProgressBarWeaponUpdated = "ultmateProgressBar.weaponUpdated";
    public const string UltimateProgressBarEnergyChanged = "ultmateProgressBar.energyChanged";



}
