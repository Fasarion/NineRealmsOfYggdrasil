using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum UpgradeValueTypes
{
    Damage,
    Cooldown,
    Size,
    Force,
    AreaEffect,
    Speed,
    Pierce,
    Amount,
    ThrustBool,
    Lifesteal,
    Unlock
}

public enum UpgradeBaseType
{
    Sword,
    Axe,
    Fireball
    
}

[CreateAssetMenu(fileName = "UpgradeObject", menuName = "Upgrades/UpgradeObject" +
                                                         "")]
public class UpgradeObject : ScriptableObject
{
    //TODO: add support for multiple upgrade values in one struct
    
    public string upgradeTitle;
    [TextArea(3, 10)]
    public string upgradeDescription;

    public Sprite upgradeSprite;

    public UpgradeBaseType thingToUpgrade;

    public UpgradeValueTypes valueToUpgrade;
    
    public float upgradeAmount;

    public bool startUnlocked;

    [HideInInspector] public int upgradeIndex;
    [HideInInspector] public bool isUnlocked;

    public List<UpgradeObject> upgradesToLock;
    public List<UpgradeObject> upgradesToUnlock;
}
