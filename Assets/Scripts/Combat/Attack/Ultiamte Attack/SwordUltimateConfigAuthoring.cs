using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class SwordUltimateConfigAuthoring : MonoBehaviour
{
    [SerializeField] private int numberOfScaledAttacks = 3;
    [SerializeField] private float scaleIncrease = 2f;
    
    class Baker : Baker<SwordUltimateConfigAuthoring>
    {
        public override void Bake(SwordUltimateConfigAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.None);
            AddComponent(entity, new SwordUltimateConfig
            {
                NumberOfScaledAttacks = authoring.numberOfScaledAttacks,
                ScaleIncrease = authoring.scaleIncrease
            });
        }
    }
}

public struct SwordUltimateConfig : IComponentData
{
    public int NumberOfScaledAttacks;
    public float ScaleIncrease;
    
    public bool IsActive;
    public int CurrentAttackCount;
}
