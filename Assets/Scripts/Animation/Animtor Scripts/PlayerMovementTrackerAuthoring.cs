using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class PlayerMovementTrackerAuthoring : MonoBehaviour
{
    class Baker : Baker<PlayerMovementTrackerAuthoring>
    {
        public override void Bake(PlayerMovementTrackerAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.None);
            AddComponent(entity, new PlayerMovementTrackerSingletonComponent());
        }
    }
}

public struct PlayerMovementTrackerSingletonComponent : IComponentData
{
    public float2 WorldSpaceInput;
    public float2 PlayerForward;
    public float2 LocalSpaceInput;
}