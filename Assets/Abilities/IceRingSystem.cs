using System.Collections;
using System.Collections.Generic;
using Damage;
using Destruction;
using Player;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;

[BurstCompile]
public partial struct IceRingSystem : ISystem
{
    private CollisionFilter _detectionFilter;
    
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<BasePlayerStatsTag>();
        state.RequireForUpdate<PhysicsWorldSingleton>();
        state.RequireForUpdate<IceRingAbility>();
        state.RequireForUpdate<IceRingConfig>();
        state.RequireForUpdate<PlayerPositionSingleton>();
        state.RequireForUpdate<PlayerSpecialAttackInput>();
        
        _detectionFilter = new CollisionFilter
        {
            BelongsTo = 1, // Projectile
            CollidesWith = 1 << 1 // Enemy
        };
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var config = SystemAPI.GetSingletonRW<IceRingConfig>();
        var input = SystemAPI.GetSingleton<PlayerSpecialAttackInput>();
        var playerPos = SystemAPI.GetSingleton<PlayerPositionSingleton>();

        foreach (var (ability, transform, chargeTimer, entity) in
                 SystemAPI.Query<RefRW<IceRingAbility>, RefRW<LocalTransform>, RefRW<ChargeTimer>>()
                     .WithEntityAccess())
        {
            if (!ability.ValueRW.isInitialized)
            {
                chargeTimer.ValueRW.maxChargeTime = config.ValueRO.maxChargeTime;
                ability.ValueRW.isInitialized = true;
            }

            chargeTimer.ValueRW.currentChargeTime += SystemAPI.Time.DeltaTime;
            if (chargeTimer.ValueRO.currentChargeTime >= config.ValueRO.maxChargeTime)
            {
                chargeTimer.ValueRW.currentChargeTime = config.ValueRO.maxChargeTime;
            }
            //TODO: Factor in player base stats into area calculation
            var tValue = chargeTimer.ValueRO.currentChargeTime / config.ValueRO.maxChargeTime;
            var area = math.lerp(0, config.ValueRO.maxArea, tValue
                );
            ability.ValueRW.area = area;

            transform.ValueRW.Scale = tValue;


            if (!input.IsHeld)
            {
                state.EntityManager.AddComponent<ShouldBeDestroyed>(entity);
                var effect = state.EntityManager.Instantiate(config.ValueRO.abilityPrefab);

                state.EntityManager.SetComponentData(effect, new LocalTransform
                {
                    Position = playerPos.Value,
                    Rotation = quaternion.identity,
                    Scale = tValue
                });
                state.EntityManager.SetComponentData(effect, new IceRingAbility
                {
                    area = area
                });
            }
        }

        foreach (var (ability, timer, buffer, transform, entity) in
                 SystemAPI.Query<RefRW<IceRingAbility>, RefRW<TimerObject>, DynamicBuffer<HitBufferElement>, RefRW<LocalTransform>>()
                     .WithEntityAccess().WithNone<ChargeTimer>())
        {
            //set up
            if (!ability.ValueRO.isInitialized)
            {
                timer.ValueRW.maxTime = config.ValueRO.maxDisplayTime;
                ability.ValueRW.isInitialized = true;
            }
            
            timer.ValueRW.currentTime += SystemAPI.Time.DeltaTime;
            
            if (timer.ValueRO.currentTime > timer.ValueRO.maxTime)
            {
                state.EntityManager.AddComponent<ShouldBeDestroyed>(entity);
            }

            if (ability.ValueRO.hasFired) return;
            
            if (timer.ValueRO.currentTime >= config.ValueRO.damageDelayTime)
            {
                var collisionWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>().CollisionWorld;
                var hits = new NativeList<DistanceHit>(state.WorldUpdateAllocator);
        
                var playerStatsEntity = SystemAPI.GetSingletonEntity<BasePlayerStatsTag>();
                var playerStatsComponent = state.EntityManager.GetComponentData<CombatStatsComponent>(playerStatsEntity);
        
                var transformLookup = SystemAPI.GetComponentLookup<LocalTransform>();
        
                float totalArea = ability.ValueRO.area;
                
                hits.Clear();
                    
                if (collisionWorld.OverlapSphere(playerPos.Value, totalArea, ref hits, _detectionFilter))
                {
                    foreach (var hit in hits)
                    {
                        var enemyPos = transformLookup[hit.Entity].Position;
                        var colPos = hit.Position;
                        float2 directionToHit = math.normalizesafe((enemyPos.xz -  transform.ValueRO.Position.xz));
                    
                        //Maybe TODO: kolla om hit redan finns i buffer
                        HitBufferElement element = new HitBufferElement
                        {
                            IsHandled = false,
                            HitEntity = hit.Entity,
                            Position = colPos,
                            Normal = directionToHit

                        };
                        buffer.Add(element);
                    }
                }
                ability.ValueRW.hasFired = true;
            }
        }
    }
}
