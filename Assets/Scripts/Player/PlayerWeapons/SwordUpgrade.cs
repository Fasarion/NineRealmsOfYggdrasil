using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

[CreateAssetMenu(fileName = "InsertUpgradeName", menuName = "Upgrades/SwordUpgrade" +
                                                            "")]
public class SwordUpgrade : UpgradeObject
{
    public List<SwordModifier> modifiers = new List<SwordModifier>();

    public enum ValueToUpgrade
    {
        Damage , 
        Force, 
        Cooldown , 
        Size , 
        Speed, 
        Lifesteal
    }

    public ValueToUpgrade valueToUpgrade;

    private void OnEnable()
    {
        baseWeaponType = BaseWeaponType.Sword;
        allUpgradeType = (AllUpgradeTypes) Enum.Parse(typeof(AllUpgradeTypes), valueToUpgrade.ToString());
    }

    public override SwordUpgrade GetSwordUpgrade()
    {
        return this;
    }

}
