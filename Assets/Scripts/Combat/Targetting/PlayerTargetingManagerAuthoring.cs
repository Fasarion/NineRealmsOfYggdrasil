using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class PlayerTargetingManagerAuthoring : MonoBehaviour
{
    [SerializeField] private GameObject TargetPrefab;
    
    class Baker : Baker<PlayerTargetingManagerAuthoring>
    {
        public override void Bake(PlayerTargetingManagerAuthoring spawnerAuthoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            
            AddComponent(entity, new PlayerTargetingPrefab
                {
                    Value = GetEntity(spawnerAuthoring.TargetPrefab, TransformUsageFlags.Dynamic)
                }
            );
            
            AddComponent(entity, new PlayerTargetInfoSingleton());
        }
    }
}

public struct PlayerTargetInfoSingleton : IComponentData
{
    public float3 LastPosition;
}

public struct PlayerTargetingPrefab : IComponentData
{
    public Entity Value;
}