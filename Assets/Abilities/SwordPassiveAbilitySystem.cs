using System.Collections;
using System.Collections.Generic;
using Destruction;
using Patrik;
using Player;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.UniversalDelegates;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;

[UpdateBefore(typeof(HandleAnimationSystem))]
public partial struct SwordPassiveAbilitySystem : ISystem
{
    private CollisionFilter _detectionFilter;
    
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<SwordPassiveAbilityConfig>();
        state.RequireForUpdate<PlayerPositionSingleton>();
        state.RequireForUpdate<GameUnpaused>();
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
        

        foreach (var (targeting, transform, swordEntity) in
                 SystemAPI.Query<RefRW<SwordTargetingComponent>, RefRW<LocalTransform>>().WithNone<ActiveWeapon>().WithEntityAccess())
        {
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
                
               var directionToTarget = math.normalizesafe(transform.ValueRO.Position - entityToFollowPos);

               float offset = targeting.ValueRO.Offset;
               transform.ValueRW.Position = entityToFollowPos + directionToTarget * offset;


                transform.ValueRW.Rotation = quaternion.LookRotation(-directionToTarget, new float3(0,1,0));
            }
            else
            {
                // Reset EntityToFollow so it can find a new target to chase
                targeting.ValueRW.EntityToFollow = Entity.Null;
            }
        }
    }
}

public partial class SwitchFollowEntityOnWeaponSwitchSystem : SystemBase
{
    protected override void OnStartRunning()
    {
        EventManager.OnWeaponSwitch += OnWeaponSwitch;
    }
    
    protected override void OnStopRunning()
    {
        EventManager.OnWeaponSwitch -= OnWeaponSwitch;
    }

    private void OnWeaponSwitch(WeaponSetupData _, List<WeaponSetupData>__)
    {
        foreach (var (sword, weapon, animatorGO, swordEntity) in
            SystemAPI.Query<SwordComponent, WeaponComponent, GameObjectAnimatorPrefab>()
                .WithEntityAccess())
        {
            bool isPassive = weapon.CurrentAttackType == AttackType.Passive;
            
            animatorGO.FollowEntity = isPassive;
        }
    }

    protected override void OnUpdate()
    {
    }
}
