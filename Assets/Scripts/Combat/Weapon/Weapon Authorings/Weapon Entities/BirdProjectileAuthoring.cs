using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class BirdProjectileAuthoring : MonoBehaviour
{
    class Baker : Baker<BirdProjectileAuthoring>
    {
        public override void Bake(BirdProjectileAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            
            AddComponent(entity, new BirdProjectileComponent());
        }
    }
}

public struct BirdProjectileComponent : IComponentData{}
