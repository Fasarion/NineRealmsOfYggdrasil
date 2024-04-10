using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class WeaponStatsConfigAuthoring : MonoBehaviour
{
    [Header("Damage")]
    [SerializeField] private float baseDamage;

    public float baseArea;
    
    class Baker : Baker<WeaponStatsConfigAuthoring>
    {
        public override void Bake(WeaponStatsConfigAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.None);
            
            AddComponent(entity, new WeaponStatsComponent
            {
                
                baseDamage = authoring.baseDamage,
                baseArea = authoring.baseArea
                
                
                
                
                
            });
        }
    }
}