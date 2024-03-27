using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerWeaponUpgradeBehaviour : MonoBehaviour
{
    [Header("Upgrade Object")]
    public UpgradeObject upgradeObject;
    
    [Header("Weapons")]
    [SerializeField] private SwordActiveWeapon sword;
    //[SerializeField] private SpearPassiveWeapon spear;
    //[SerializeField] private HammerPassiveWeapon hammer;
    //[SerializeField] private BowPassiveWeapon bow;
    //[SerializeField] private ShurikenPassiveWeapon shuriken;
    
    // [Header("Weapon Upgrades")]
    // [SerializeField] private SwordUpgrade swordUpgrade; 
    // [SerializeField] private SpearUpgrade spearUpgrade;
    // [SerializeField] private HammerUpgrade hammerUpgrade;
    // [SerializeField] private BowUpgrade bowUpgrade;
    // [SerializeField] private ShurikenUpgrade shurikenUpgrade;

    private BaseWeaponType baseWeaponType;

    [Header("Weapon Lists")]
    public List<BaseWeaponType> activeWeapons = new List<BaseWeaponType>();
    public List<GameObject> playerWeapons = new List<GameObject>();
    
    private Dictionary<BaseWeaponType, Weapon> weaponTypeDictionary = new Dictionary<BaseWeaponType, Weapon>();

    // cached property index for animator string lookup for efficiency
    private static readonly int WeaponAttackSpeedAnimId = Animator.StringToHash("weaponAttackSpeed");

    private void Awake()
    {
        GetWeaponReferences();
        FillPlayerWeaponsList();
        SetupWeaponTypeDictionary();
    }

    private void Start()
    {
        InactivatePlayerWeapons();
        activeWeapons.Clear();
        
        ActivateWeapon(BaseWeaponType.Sword, sword.gameObject);
    }

    private void GetWeaponReferences()
    {
        playerWeapons.Clear();

        sword = FindObjectOfType<SwordActiveWeapon>();
        //spear = FindObjectOfType<SpearPassiveWeapon>();
        //hammer = FindObjectOfType<HammerPassiveWeapon>();
        //bow = FindObjectOfType<BowPassiveWeapon>();
        //shuriken = FindObjectOfType<ShurikenPassiveWeapon>();
    }
    
    private void FillPlayerWeaponsList()
    {
        playerWeapons.Add(sword.gameObject);
        //playerWeapons.Add(spear.gameObject);
        //playerWeapons.Add(hammer.gameObject);
        //playerWeapons.Add(bow.gameObject);
        //playerWeapons.Add(shuriken.gameObject);
    }
    
    private void SetupWeaponTypeDictionary()
    {
        weaponTypeDictionary = new Dictionary<BaseWeaponType, Weapon>
        {
            {BaseWeaponType.Sword, sword},
            //{BaseWeaponType.Spear, spear},
            //{BaseWeaponType.Hammer, hammer},
            //{BaseWeaponType.Bow, bow},
            //{BaseWeaponType.Shuriken, shuriken},
        };
    }

    private void InactivatePlayerWeapons()
    {
        foreach (var weapon in playerWeapons)
        {
            weapon.SetActive(false);
        }
    }

    private void ActivateWeapon(BaseWeaponType weaponType, GameObject weapon)
    {
        activeWeapons.Add(weaponType);
        weapon.SetActive(true);
    }

    public void RecieveUpgrade(UpgradeObject upgrade)
    {
        baseWeaponType = upgrade.baseWeaponType;
        upgradeObject = upgrade;

        TryActivateWeapon(baseWeaponType);

        var weapon = weaponTypeDictionary[baseWeaponType];
        weapon.SortUpgradePackage(upgrade);
        ApplyWeaponUpgrade(weapon, upgrade);
        
        {
            // // TODO: clean this up
            // switch (baseWeaponType)
            // {
            //     case BaseWeaponType.Sword: 
            //         swordUpgrade = upgrade.GetSwordUpgrade();
            //         ApplySwordUpgrade(swordUpgrade);
            //         break;
            //     
            //     case BaseWeaponType.Hammer:
            //         hammerUpgrade = upgrade.GetHammerUpgrade();
            //         ApplyHammerUpgrade(hammerUpgrade);
            //         break;
            //         
            //     case BaseWeaponType.Bow:
            //         bowUpgrade = upgrade.GetBowUpgrade();
            //         ApplyBowUpgrade(bowUpgrade);
            //         break;
            //     
            //     case BaseWeaponType.Spear:
            //         spearUpgrade = upgrade.GetSpearUpgrade();
            //         ApplySpearUpgrade(spearUpgrade);
            //         break;
            //     
            //     case BaseWeaponType.Shuriken:
            //         shurikenUpgrade = upgrade.GetShurikenUpgrade();
            //         ApplyShurikenUpgrade(shurikenUpgrade);
            //         break;
            // }
        }
        
    }

    private bool TryActivateWeapon(BaseWeaponType baseWeaponType)
    {
        if (!activeWeapons.Contains(baseWeaponType))
        {
            ActivateWeapon(baseWeaponType, weaponTypeDictionary[baseWeaponType].gameObject);
            return true;
        }

        return false;
    }
    
    private void ApplyWeaponUpgrade(Weapon weapon, UpgradeObject upgrade)
    {
        float upgradeAmount = upgrade.upgradeAmount;
        
        switch (upgrade.allUpgradeType)
        {
            case AllUpgradeTypes.Damage:
                if (weapon is IMakeDamage makeDamage) makeDamage.UpdateDamage((int)upgradeAmount);
                break;
            
            // NOTE: updates with positive value
            case AllUpgradeTypes.Cooldown:
                if (weapon is IHasCooldown iCooldown) iCooldown.UpdateCooldown(upgradeAmount);
                break;
        
            case AllUpgradeTypes.Size:
                if (weapon is IHasSize iSize) iSize.UpdateSize(upgradeAmount);
                break;
            
            case AllUpgradeTypes.Speed:
                if (weapon is IHasSpeed iSpeed) iSpeed.UpdateSpeed(upgradeAmount);
                break;
            
            case AllUpgradeTypes.AreaEffect:
                if (weapon is IHasArea iArea) iArea.UpdateArea(upgradeAmount);
                break;
            
            case AllUpgradeTypes.Force:
                if (weapon is IHasForce iForce) iForce.UpdateForce(upgradeAmount);
                break;
            
            case AllUpgradeTypes.ThrustBool:
                if (weapon is IHasTrustBool iTrustBool) iTrustBool.UpdateTrustBool(upgradeAmount);
                break;
            
            case AllUpgradeTypes.Pierce:
                if (weapon is IHasPierce iPierce) iPierce.UpdatePierce(upgradeAmount);
                break;
            
            case AllUpgradeTypes.Amount:
                if (weapon is IHasAmount iAmount) iAmount.UpdateAmount(upgradeAmount);
                break;
            
            case AllUpgradeTypes.Lifesteal:
                if (weapon is IHasLifeSteal iLifeSteal) iLifeSteal.UpdateLifeSteal(upgradeAmount);
                break;
            
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    // private void ApplySwordUpgrade(SwordUpgrade upgrade)
    // {
    //     //sword.SortUpgradePackage(upgrade);
    //     
    //     switch (upgrade.valueToUpgrade)
    //     {
    //         case SwordUpgrade.ValueToUpgrade.Damage:
    //             sword.damage += (int)upgrade.upgradeAmount;
    //             break;
    //         
    //         case SwordUpgrade.ValueToUpgrade.Cooldown:
    //             sword.cooldown -= upgrade.upgradeAmount;
    //             break;
    //         
    //         case SwordUpgrade.ValueToUpgrade.Force:
    //             sword.force += upgrade.upgradeAmount;
    //             break;
    //         
    //         case SwordUpgrade.ValueToUpgrade.Size:
    //             sword.SetSize(upgrade.upgradeAmount);
    //             break;
    //         
    //         case SwordUpgrade.ValueToUpgrade.Speed:
    //             sword.SetSpeed(upgrade.upgradeAmount);
    //             break;
    //         
    //         case SwordUpgrade.ValueToUpgrade.Lifesteal:
    //             sword.lifesteal += upgrade.upgradeAmount;
    //             break;
    //     }
    // }
    //
    // private void ApplyHammerUpgrade(HammerUpgrade upgrade)
    // {
    //    // hammer.SortUpgradePackage(upgrade);
    //     
    //     switch (upgrade.valueToUpgrade)
    //     {
    //         case HammerUpgrade.ValueToUpgrade.Damage:
    //             hammer.damage += (int)upgrade.upgradeAmount;
    //             break;
    //         
    //         case HammerUpgrade.ValueToUpgrade.Cooldown:
    //             //hammer.cooldown -= upgrade.upgradeAmount;
    //             var speedValue = hammer.hammerAnimator.GetFloat(WeaponAttackSpeedAnimId);
    //             speedValue += upgrade.upgradeAmount;
    //             hammer.hammerAnimator.SetFloat(WeaponAttackSpeedAnimId, speedValue);
    //             break;
    //         
    //         case HammerUpgrade.ValueToUpgrade.Force: 
    //             hammer.force += upgrade.upgradeAmount;
    //             break;
    //         
    //         case HammerUpgrade.ValueToUpgrade.Size:
    //             hammer.size += upgrade.upgradeAmount;
    //             break;
    //         
    //         case HammerUpgrade.ValueToUpgrade.AreaEffect:
    //             hammer.area += upgrade.upgradeAmount;
    //             break;
    //     }
    // }
    //
    // private void ApplyBowUpgrade(BowUpgrade upgrade)
    // {
    //
    //     //bow.SortUpgradePackage(upgrade);
    //
    //     switch (upgrade.valueToUpgrade)
    //     {
    //         case BowUpgrade.ValueToUpgrade.Damage:
    //             bow.damage += (int)upgrade.upgradeAmount;
    //             break;
    //         
    //         case BowUpgrade.ValueToUpgrade.Cooldown:
    //             //bow.cooldown -= upgrade.upgradeAmount;
    //             var speedValue = bow.bowAnimator.GetFloat(WeaponAttackSpeedAnimId);
    //             speedValue += upgrade.upgradeAmount;
    //             bow.bowAnimator.SetFloat(WeaponAttackSpeedAnimId, speedValue);
    //             break;
    //         
    //         case BowUpgrade.ValueToUpgrade.Pierce:
    //             bow.force += upgrade.upgradeAmount;
    //             break;
    //         
    //         case BowUpgrade.ValueToUpgrade.Size:
    //             bow.size += upgrade.upgradeAmount;
    //             break;
    //         
    //         case BowUpgrade.ValueToUpgrade.Speed:
    //             bow.speed += upgrade.upgradeAmount;
    //             break;
    //         
    //         case BowUpgrade.ValueToUpgrade.Amount:
    //             bow.amount += upgrade.upgradeAmount;
    //             break;
    //     }
    // }
    //
    // private void ApplySpearUpgrade(SpearUpgrade upgrade)
    // {
    //     //spear.SortUpgradePackage(upgrade);
    //     
    //     switch (upgrade.valueToUpgrade)
    //     {
    //         case SpearUpgrade.ValueToUpgrade.Damage:
    //             spear.damage += (int)upgrade.upgradeAmount;
    //             break;
    //         
    //         case SpearUpgrade.ValueToUpgrade.Cooldown:
    //             //spear.spearAnimator.speed += upgrade.upgradeAmount;
    //             var speedValue = spear.spearAnimator.GetFloat(WeaponAttackSpeedAnimId);
    //             speedValue += upgrade.upgradeAmount;
    //             spear.spearAnimator.SetFloat(WeaponAttackSpeedAnimId, speedValue);                
    //             break;
    //         
    //         case SpearUpgrade.ValueToUpgrade.Force:
    //             spear.force += upgrade.upgradeAmount;
    //             break;
    //         
    //         case SpearUpgrade.ValueToUpgrade.Size:
    //             spear.size += upgrade.upgradeAmount;
    //             break;
    //         
    //         case SpearUpgrade.ValueToUpgrade.Speed:
    //             spear.speed += upgrade.upgradeAmount;
    //             break;
    //         
    //         case SpearUpgrade.ValueToUpgrade.ThrustBool:
    //             if(upgrade.upgradeAmount > 0) spear.SetThrustMode(true);
    //             else spear.SetThrustMode(false);
    //             break;
    //     }
    // }
    //
    // private void ApplyShurikenUpgrade(ShurikenUpgrade upgrade)
    // {
    //    // shuriken.SortUpgradePackage(upgrade);
    //     
    //     switch (upgrade.valueToUpgrade)
    //     {
    //         case ShurikenUpgrade.ValueToUpgrade.Damage:
    //             shuriken.damage += (int)upgrade.upgradeAmount;
    //             break;
    //         
    //         case ShurikenUpgrade.ValueToUpgrade.Speed:
    //             shuriken.speed += upgrade.upgradeAmount;
    //             break;
    //         
    //         case ShurikenUpgrade.ValueToUpgrade.Force:
    //             shuriken.force += upgrade.upgradeAmount;
    //             break;
    //         
    //         case ShurikenUpgrade.ValueToUpgrade.Size:
    //             shuriken.size += upgrade.upgradeAmount;
    //             break;
    //         
    //         case ShurikenUpgrade.ValueToUpgrade.Amount:
    //             shuriken.ShurikenCount = (int)upgrade.upgradeAmount;
    //             break;
    //     }
    // }
    //
}
