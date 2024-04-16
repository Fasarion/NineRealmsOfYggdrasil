using Patrik;
using UnityEngine;

[System.Serializable]
public struct CombatStats
{
    [Header("Damage")]
    public float BaseDamage;
    public ComboMultiplier AttackComboMultiplier;

    [Header("Critical")]
    public float CriticalMultiplier;
    [Range(0,1f)]
    public float CriticalRate;
    
    [Header("Area")]
    public float Area;
    
    [Header("Speed")]
    public float AttackSpeed;

    [Header("KnockBack")] 
    public float BaseKnockBack;
    public ComboMultiplier KnockBackComboMultiplier;
    
    [Header("Cooldown")]
    public float Cooldown;
    
    [Header("Energy")]
    public float MaxEnergy;
    public float EnergyFillPerHit;

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
    
    public static float GetTotalDamageWithCrit(CombatStatsComponent stats1, CombatStatsComponent stats2, AttackType attackType, float randomFloat, int combo = 0)
    {
        float uncritDamage = GetTotalDamageWithoutCrit(stats1, stats2, attackType, combo);
        float critMultiplier = GetCriticalMultiplier(stats1, stats2, attackType, randomFloat);

        return uncritDamage * critMultiplier;
    }

    private static float GetCriticalMultiplier(CombatStatsComponent stats1, CombatStatsComponent stats2, AttackType attackType, float randomFloat)
    {
        var critRate1 = GetCritRate(stats1, attackType, out float critDamage1);
        var critRate2 = GetCritRate(stats2, attackType, out float critDamage2);
        float totalCritRate = critRate1 + critRate2;
        
        bool applyCrit = totalCritRate > randomFloat;
        if (applyCrit)
        {
            float totalCritDamage = critDamage1 + critDamage2;
            return totalCritDamage * totalCritRate;
        }
        else
        {
            return 1;
        }
    }

    private static float GetCritRate(CombatStatsComponent stats1, AttackType attackType, out float critDamage)
    {
        CombatStats attackStats = GetAttackStatsFromComponent(stats1, attackType);
        var critRate1 = attackStats.CriticalRate;
        critDamage = attackStats.CriticalMultiplier;
        return critRate1;
    }

    static float GetTotalDamageWithoutCrit(CombatStatsComponent stats1, CombatStatsComponent stats2, AttackType attackType, int combo = 0)
    {
        return GetTotalDamageWithoutCrit(stats1, attackType, combo) + GetTotalDamageWithoutCrit(stats2, attackType, combo);
    }

    static float GetTotalDamageWithoutCrit(CombatStatsComponent stats, AttackType attackType, int combo = 0)
    {
        CombatStats attackStats = GetAttackStatsFromComponent(stats, attackType);
        float totalDamage = stats.OverallStats.BaseDamage 
                            * attackStats.BaseDamage
                            * attackStats.AttackComboMultiplier.GetCombo(combo);

        return totalDamage;
    }
    
    public static float GetTotalKnockBack(CombatStatsComponent stats1, CombatStatsComponent stats2, AttackType attackType, int combo)
    {
        return GetTotalKnockBack(stats1, attackType, combo) + GetTotalKnockBack(stats2, attackType, combo);
    }

    private static float GetTotalKnockBack(CombatStatsComponent stats1, AttackType attackType, int combo)
    {
        CombatStats attackStats = GetAttackStatsFromComponent(stats1, attackType);
        float knockBack = stats1.OverallStats.BaseKnockBack
                          * attackStats.BaseKnockBack
                          * attackStats.KnockBackComboMultiplier.GetCombo(combo);
        return knockBack;
    }
}