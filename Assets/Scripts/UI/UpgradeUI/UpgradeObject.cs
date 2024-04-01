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
    Sword
}

public class UpgradeObject : ScriptableObject
{
    public string upgradeTitle;
    [TextArea(3, 10)]
    public string upgradeDescription;

    public Image upgradeImage;

    public UpgradeBaseType thingToUpgrade;

    public UpgradeValueTypes valueToUpgrade;
    
    public float upgradeAmount;

    [HideInInspector] public int upgradeIndex;
    [HideInInspector] public bool isUnlocked;

    public List<UpgradeObject> upgradesToLock;
    public List<UpgradeObject> upgradesToUnlock;
}
