using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Serialization;

public class IsDyingComponentAuthoring : MonoBehaviour
{
    public float excessDamage;
    public bool isHandled;

    public class IsDyingComponentAuthoringBaker : Baker<IsDyingComponentAuthoring>
    {
        public override void Bake(IsDyingComponentAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity,
                new IsDyingComponent
                {
                    ExcessDamage = authoring.excessDamage, IsHandled = authoring.isHandled
                });
        }
    }
}

public struct IsDyingComponent : IComponentData
{
    public float ExcessDamage;
    public bool IsHandled;
}
