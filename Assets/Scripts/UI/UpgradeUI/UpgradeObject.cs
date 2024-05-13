using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public struct UpgradeInformation
{
    [Tooltip("The base objects or groups of objects to modify. Each type added will be modified. You can add as many items to the list as you want!")]
    public UpgradeBaseType thingToUpgrade;
    [Tooltip("The specific type of values being added (or subtracted) to. Each type added will be modified. You can add as many types to the list as you want!")]
    public UpgradeValueTypes valueToUpgrade;
    [Tooltip("The actual numbers that will be added (or subtracted). If this list is the same length as ValuesToUpgrade each number " +
             "item will modify the value item with the same index. If this list is shorter than the amount of ValuesToUpgrade the last " +
             "element in the list will be used as a default. If this list is longer than ValuesToUpgrade the last items in this list will be ignored.")]
    public float valueAmount;
}

public enum UpgradeValueTypes
{
    baseAtk,
    energyRegen, //this is in %
    attackSpeed,
    defence,
    areaEffect,
    chargeSpeed,
    hp,
    movementSpeed,
    cooldown,
    crit,
    reactionDamage,
    reactionArea, //Hur stort område reactionerna (combustion, lightningchain etc) gör skada
    damage, //this is in % on player
    lightningchains,
    damageModifier,
    
    // skill modifiers
    normalModifier,
    specialModifier,
    ultimateModifier,
    passiveModifier,
    
    //elemental 
    applyFire,
    applyLightning,
    applyIce,
    
    Unlock,
    knockbackForce,
    hitStopDuration,
}

public enum UpgradeBaseType
{
    Sword,
    SwordSpecialAbility,
    SwordUltimateAbility,
    SwordPassiveAbility,
    
    Hammer,
    HammerSpecialAbility,
    HammerUltimateAbility,
    HammerPassiveAbility,
    
    Birds,
    Mead,
    Player,
}

[CreateAssetMenu(fileName = "UpgradeObject", menuName = "Upgrades/UpgradeObject" +
                                                         "")]
public class UpgradeObject : ScriptableObject
{
    //TODO: add support for multiple upgrade values in one struct
    [Header("--Upgrade Card Aesthetics--")]

    [Tooltip("The text at the top of the upgrade card")]
    public string upgradeTitle;
    [Tooltip("The sprite (image) of the upgrade card")]
    public Sprite upgradeSprite;
    [Tooltip("The main text box of the upgrade card")]
    [TextArea(3, 10)]
    public string upgradeDescription;

    [Header("")] [Header("--Upgrade Modifiers--")]

    public List<UpgradeInformation> upgrades;
    

    [Header("")]
    [Header("--Upgrade Tree Logic--")]

    [Tooltip("If this is checked the upgrade will be available in the first batch of possible upgrades during the game.")]
    public bool startUnlocked;

    [HideInInspector] public int upgradeIndex;
    [HideInInspector] public bool isUnlocked;

    [Tooltip("A list of all upgrades that will be locked when the player picks this upgrade. You can add as many items (UpgradeObjects) as you want to this list!")]
    public List<UpgradeObject> upgradesToLock;
    [Tooltip("A list of all upgrades that will be unlocked when the player picks this upgrade. You can add as many items (UpgradeObjects) as you want to this list!")]
    public List<UpgradeObject> upgradesToUnlock;
}
