using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class BirdnadoAbilityAuthoring : MonoBehaviour
{
    class Baker : Baker<BirdnadoAbilityAuthoring>
    {
        public override void Bake(BirdnadoAbilityAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new BirdnadoComponent());
        }
    }
}

public struct BirdnadoComponent : IComponentData{}
