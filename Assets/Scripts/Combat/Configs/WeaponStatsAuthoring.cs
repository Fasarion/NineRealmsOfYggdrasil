using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class WeaponStatsAuthoring : MonoBehaviour 
{
    [Header("Overall Stats")] 
    public CombatStats OverallStats = CombatStats.Default; 
    
    [Header("Attack Stats")]
    public CombatStats NormalAttack = CombatStats.Default;
    [Space]
    public CombatStats SpecialAttack = CombatStats.Default;
    [Space]
    public CombatStats PassiveAttack = CombatStats.Default;
    [Space]
    public CombatStats UltimateAttack = CombatStats.Default;
    
   
   class Baker : Baker<WeaponStatsAuthoring>
   {
       public override void Bake(WeaponStatsAuthoring authoring)
       {
           var entity = GetEntity(TransformUsageFlags.None);
           
           AddComponent(entity, new CombatStatsComponent()
           {
               OverallStats = authoring.OverallStats,
               NormalAttackStats = authoring.NormalAttack,
               SpecialAttackStats = authoring.SpecialAttack,
               UltimateAttackStats = authoring.UltimateAttack,
               PassiveAttackStats = authoring.PassiveAttack,
           });
       }
   }
}

public struct CombatStatsComponent : IComponentData
{
    
    
    public CombatStats OverallStats;
    
    public CombatStats NormalAttackStats;
    public CombatStats SpecialAttackStats;
    public CombatStats UltimateAttackStats;
    public CombatStats PassiveAttackStats;
    
    
}



[System.Serializable]
public struct ComboMultiplier
{
    // TODO: Make List?
    public float AttackOne;
    public float AttackTwo;
    public float AttackThree;
    
    public static readonly ComboMultiplier Default = new ComboMultiplier
    {
        AttackOne = 1,
        AttackTwo = 1,
        AttackThree = 1,
    };

    public float GetCombo(int index = 0)
    {
        switch (index)
        {
            case 0:
                return AttackOne;
            
            case 1:
                return AttackTwo;
            
            case 2:
                return AttackThree;
            
            default:
                return 1;
        }
    }
}