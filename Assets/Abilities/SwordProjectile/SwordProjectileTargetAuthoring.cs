using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class SwordProjectileTargetAuthoring : MonoBehaviour
{
    public class SwordProjectileTargetBaker : Baker<SwordProjectileTargetAuthoring>
    {
        public override void Bake(SwordProjectileTargetAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new SwordProjectileTarget());
        }
    }
}

public struct SwordProjectileTarget : IComponentData
{
}
