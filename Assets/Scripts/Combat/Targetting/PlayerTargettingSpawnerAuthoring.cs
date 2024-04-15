using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class PlayerTargettingSpawnerAuthoring : MonoBehaviour
{
    [SerializeField] private GameObject TargetPrefab;
    
    class Baker : Baker<PlayerTargettingSpawnerAuthoring>
    {
        public override void Bake(PlayerTargettingSpawnerAuthoring spawnerAuthoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            
            AddComponent(entity, new PlayerTargetingPrefab
                {
                    Value = GetEntity(spawnerAuthoring.TargetPrefab, TransformUsageFlags.Dynamic)
                }
            );
        }
    }
}

public struct PlayerTargetingPrefab : IComponentData
{
    public Entity Value;
}