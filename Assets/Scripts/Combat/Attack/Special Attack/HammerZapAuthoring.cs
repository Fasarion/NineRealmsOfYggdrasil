using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class HammerZapAuthoring : MonoBehaviour
{
    class Baker : Baker<HammerZapAuthoring>
    {
        public override void Bake(HammerZapAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new HammerZapComponent());
        }
    }
}

public struct HammerZapComponent : IComponentData{}
