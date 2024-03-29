using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Player
{
    public class PlayerAuthoring : MonoBehaviour
    {
        class Baker : Baker<PlayerAuthoring>
        {
            public override void Bake(PlayerAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent<PlayerTag>();
                AddComponent<PlayerPositionSingleton>();
            }
        }
    }

    public struct PlayerPositionSingleton : IComponentData
    {
        public float3 Value;
    }

}

