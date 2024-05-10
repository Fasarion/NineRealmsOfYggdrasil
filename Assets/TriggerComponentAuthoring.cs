using System.Collections;
using System.Collections.Generic;
using Damage;
using Unity.Entities;
using UnityEngine;

public class TriggerComponentAuthoring : MonoBehaviour
{
    
     class Baker : Baker<TriggerComponentAuthoring>
    {
        public override void Bake(TriggerComponentAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new TriggerComponent());
        }
    }
}

public struct TriggerComponent : IComponentData{}
