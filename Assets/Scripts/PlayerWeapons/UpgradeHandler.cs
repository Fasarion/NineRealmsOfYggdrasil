using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;


[SuppressMessage("ReSharper", "IdentifierTypo")]
public enum BaseWeaponType { Sword, Bow, Spear, Hammer, Shuriken }
public enum RarityType { Common, Uncommon, Rare, Epic, Legendary }
public enum CardType { UpgradeSingle, UpgradeDouble, Evolution, Weapon }

public enum LevelUpType { Upgrade, Modification, BaseWeapon, Evolution }




[CreateAssetMenu(fileName = "UpgradeHandler", menuName = "EventHandlers/UpgradeHandler" +
    "")]
public class UpgradeHandler : ScriptableObject
{
    public event Action<UpgradeObject> onUpgradeChosen;
    
    public void UpgradeChosen(UpgradeObject upgrade)
    {
        onUpgradeChosen?.Invoke(upgrade);
    }
}
