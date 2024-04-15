using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class PlayerTargettingAuthoring : MonoBehaviour
{
    class Baker : Baker<PlayerTargettingAuthoring>
    {
        public override void Bake(PlayerTargettingAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            
            AddComponent(entity, new PlayerTargetingComponent());
        }
    }
}

public struct PlayerTargetingComponent:IComponentData{}
