using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class BirdPassiveAttackConfigAuthoring : MonoBehaviour
{
    [SerializeField] private int birdCount = 2;
    
    class Baker : Baker<BirdPassiveAttackConfigAuthoring>
    {
        public override void Bake(BirdPassiveAttackConfigAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.None);
            AddComponent(entity, new BirdPassiveAttackConfig
            {
                BirdCount = authoring.birdCount
            });
        }
    }
}

public struct BirdPassiveAttackConfig : IComponentData
{
    public int BirdCount;
}
