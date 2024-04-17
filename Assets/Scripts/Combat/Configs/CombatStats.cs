using Patrik;
using Unity.Mathematics;
using UnityEngine;

[System.Serializable]
public struct CombatStatValue
{
    public float BaseValue;
    public ComboMultiplier Multiplier;

    public static readonly CombatStatValue Default = new CombatStatValue
    {
        BaseValue = 1,
        Multiplier = ComboMultiplier.Default
    };

    public static float GetTotalStatValue(CombatStatValue stat, int combo)
    {
        return stat.BaseValue * stat.Multiplier.GetCombo(combo);
    }
}

public enum CombatStatType
{
     Damage,
     CriticalRate,
     CriticalModifier,
    
     Area,
    
     AttackSpeed,

     KnockBack,
     Cooldown,
     EnergyFillPerHit,
}

[System.Serializable]
public struct CombatStats
{
    [Header("Damage")] 
    public CombatStatValue Damage;

    [Header("Critical")]
    public CombatStatValue CriticalRate;
    public CombatStatValue CriticalModifier;
    
    [Header("Area")]
    public CombatStatValue Area;
    
    [Header("Speed")]
    public CombatStatValue AttackSpeed;

    [Header("KnockBack")] 
    public CombatStatValue KnockBack;
    
    [Header("Cooldown")]
    public CombatStatValue Cooldown;
    
    [Header("Energy")]
    public CombatStatValue EnergyFillPerHit;

    public CombatStatValue GetStatValueFromType(CombatStatType type)
    {
        switch (type)
        {
               case CombatStatType.Damage : return Damage;
               case CombatStatType.CriticalRate : return CriticalRate;
               case CombatStatType.CriticalModifier : return CriticalModifier;
               case CombatStatType.Area : return Area;
               case CombatStatType.AttackSpeed : return AttackSpeed;
               case CombatStatType.KnockBack : return KnockBack;
               case CombatStatType.Cooldown : return Cooldown;
               case CombatStatType.EnergyFillPerHit : return EnergyFillPerHit;
        }

        return default;
    }
    

    public static readonly CombatStats Default = new CombatStats
    {
        Damage = CombatStatValue.Default,
        
        CriticalRate = new CombatStatValue
        {
            BaseValue = 0, // avoid having 100% crit chance per default
            Multiplier = ComboMultiplier.Default
        },
            
        CriticalModifier = CombatStatValue.Default, 
        
        Area = CombatStatValue.Default,
        
        AttackSpeed = CombatStatValue.Default,

        KnockBack = CombatStatValue.Default,
        
        Cooldown = CombatStatValue.Default,
        EnergyFillPerHit = CombatStatValue.Default
    };

    private static CombatStats GetAttackStatsFromComponent(CombatStatsComponent stats, AttackType attackType)
    {
        switch (attackType)
        {
            case AttackType.Normal:
                return stats.NormalAttackStats;

            case AttackType.Special:
                return stats.SpecialAttackStats;
            
            case AttackType.Ultimate:
                return stats.UltimateAttackStats;
            
            case AttackType.Passive:
                return stats.PassiveAttackStats;
        }
        
        return default;
    }

    /// <summary>
    /// Gets the total stat value from two CombatStatsComponents for a specific combat stat.
    /// </summary>
    /// <param name="stats1"> First stat component to get value from. </param>
    /// <param name="stats2"> Second stat component to get value from. </param>
    /// <param name="attackType"> What kind of attack that is performed. </param>
    /// <param name="combatStatType"> Which combat stat to retrieve. </param>
    /// <param name="combo"> In what combo the attack was performed. </param>
    /// <returns></returns>
    public static float GetCombinedStatValue(CombatStatsComponent stats1, CombatStatsComponent stats2, AttackType attackType, CombatStatType combatStatType, int combo)
    {
        return GetStatValue(stats1, attackType, combatStatType, combo) +
               GetStatValue(stats2, attackType, combatStatType, combo);
    }

    static float GetStatValue(CombatStatsComponent statsComponent, AttackType attackType, CombatStatType statType, int combo)
    {
        CombatStats attackStats = GetAttackStatsFromComponent(statsComponent, attackType);
        
        float overallStat = CombatStatValue.GetTotalStatValue(statsComponent.OverallStats.GetStatValueFromType(statType), combo);
        float attackStat = CombatStatValue.GetTotalStatValue(attackStats.GetStatValueFromType(statType), combo);
        
        return overallStat + attackStat;
    }
}