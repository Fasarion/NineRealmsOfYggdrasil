using UnityEngine;

public class WeaponStatsSuggestionAuthoring : MonoBehaviour
{
    [Header("Overall Stats")] 
    public CombatStats OverallStats;
    
    [Header("Attack Stats")]
    public CombatStats NormalAttack;
    [Space]
    public CombatStats SpecialAttack;
    [Space]
    public CombatStats PassiveAttack;
}

[System.Serializable]
public struct CombatStats
{
    [Header("Damage")]
    public float BaseDamage;
    public AttackComboMultiplier AttackComboMultiplier; // kanske inte behövs?

    [Header("Critical")]
    public float CriticalDamage;
    public float CriticalRate;
    
    [Header("Area")]
    public float Area;
    
    [Header("Speed")]
    public float AttackSpeed;
    
    [Header("Cooldown")]
    public float Cooldown;
    
    [Header("Energy")]
    public float MaxEnergy;
    public float EnergyFillPerHit;
}

[System.Serializable]
public struct AttackComboMultiplier
{
    // TODO: Make List?
    public float AttackOne;
    public float AttackTwo;
    public float AttackThree;
}