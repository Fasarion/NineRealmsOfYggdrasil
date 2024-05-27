using System.Collections;
using System.Collections.Generic;
using Destruction;
using Player;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;

public partial struct SwordPassiveAbilitySystem : ISystem
{
    private CollisionFilter _detectionFilter;
    
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<SwordPassiveAbilityConfig>();
        state.RequireForUpdate<PlayerPositionSingleton>();
        _detectionFilter = new CollisionFilter
        {
            BelongsTo = 1, // Projectile
            CollidesWith = 1 << 1 // Enemy
        };
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var playerPos = SystemAPI.GetSingleton<PlayerPositionSingleton>();
        var config = SystemAPI.GetSingleton<SwordPassiveAbilityConfig>();
        
        foreach (var (targeting, transform, animatorGO, swordEntity) in
                 SystemAPI.Query<RefRW<SwordTargetingComponent>, RefRW<LocalTransform>, GameObjectAnimatorPrefab>().WithNone<ActiveWeapon>().WithEntityAccess())
        {
            animatorGO.FollowEntity = true; 
            
            if (!state.EntityManager.Exists(targeting.ValueRO.EntityToFollow))
            {
                                var collisionWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>().CollisionWorld;
                var hits = new NativeList<DistanceHit>(state.WorldUpdateAllocator);

                float totalArea = config.Radius;

                float3 originPosition = playerPos.Value;

                hits.Clear();
                
                if (collisionWorld.OverlapSphere(originPosition, totalArea,
                        ref hits, _detectionFilter))
                {
                    foreach (var hit in hits)
                    {
                        targeting.ValueRW.EntityToFollow = hit.Entity;

                        break;
                    }
                }
            }
            
            if (state.EntityManager.HasComponent<LocalTransform>(targeting.ValueRO.EntityToFollow))
            {
                var entityToFollowTransform = state.EntityManager.GetComponentData<LocalTransform>(targeting.ValueRO.EntityToFollow);
                var entityToFollowPos = entityToFollowTransform.Position;
                transform.ValueRW.Position = entityToFollowPos + new float3(targeting.ValueRO.Offset);
            }
        }
    }
}

public partial class SwitchFollowEntityOnWeaponSwitchSystem
{
    
}
