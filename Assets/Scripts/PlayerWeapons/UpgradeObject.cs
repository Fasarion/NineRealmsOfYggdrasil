using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AllUpgradeTypes
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
    Lifesteal
}

public class UpgradeObject : ScriptableObject
{
    [HideInInspector] public BaseWeaponType baseWeaponType;
    public RarityType rarityType;
    public CardType cardType;
    
    
    [TextArea(3, 10)]
    public string upgradeFlavor;
    [TextArea(3, 10)]
    public string upgradeDescription;

    public AllUpgradeTypes allUpgradeType { get; protected set; }

    public UpgradeTreeObject upgradeTree;
    
    public List<ActionSequenceObject> AttackActions = new List<ActionSequenceObject>();
    public List<ActionSequenceObject> SwingActions = new List<ActionSequenceObject>();
    public List<ActionSequenceObject> ImpactActions = new List<ActionSequenceObject>();
    public List<ActionSequenceObject> HitActions = new List<ActionSequenceObject>();
    public List<ActionSequenceObject> RetractActions = new List<ActionSequenceObject>();
    public List<ActionSequenceObject> PermanentModifiers = new List<ActionSequenceObject>();
    public List<ActionSequenceObject> PowerActions = new List<ActionSequenceObject>();
    public List<ActionSequenceObject> EndActions = new List<ActionSequenceObject>();
    
    public bool overrideSwing;
    public bool overrideImpact;
    public bool overrideHit;
    public bool overrideRetract;
    public bool overridePermanent;
    public bool overridePower;
    
    public float upgradeAmount;

    public virtual SwordUpgrade GetSwordUpgrade()
    {
        return null;
    }
}
