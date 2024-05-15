using System.Collections;
using System.Collections.Generic;
using Damage;
using Destruction;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;

[BurstCompile]
[UpdateAfter(typeof(ThunderBoltAbilitySystem))]
[UpdateBefore(typeof(HitStopSystem))]
[UpdateBefore(typeof(AddDamageBufferElementOnTriggerSystem))]
[UpdateBefore(typeof(ApplyDamageSystem))]
[UpdateInGroup(typeof(CombatSystemGroup))]
public partial struct ThunderBoltProjectileSystem : ISystem
{
    private CollisionFilter _detectionFilter;
    
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<PhysicsWorldSingleton>();
        state.RequireForUpdate<ThunderBoltConfig>();
        state.RequireForUpdate<ThunderBoltProjectile>();
        _detectionFilter = new CollisionFilter
        {
            BelongsTo = 1, // Projectile
            CollidesWith = 1 << 1 // Enemy
        };
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var config = SystemAPI.GetSingleton<ThunderBoltConfig>();
        var ecb = new EntityCommandBuffer(state.WorldUpdateAllocator);

        foreach (var (ability, timer, transform, entity) in
                 SystemAPI.Query<RefRW<ThunderBoltProjectile>, RefRW<TimerObject>, RefRW<LocalTransform>>()
                     .WithEntityAccess())
        {
            timer.ValueRW.currentTime += SystemAPI.Time.DeltaTime;

            if (timer.ValueRO.currentTime > config.MaxDisplayTime)
            {
                ecb.AddComponent<ShouldBeDestroyed>(entity);
            }
            
            if (ability.ValueRO.HasFired) continue;

            if (timer.ValueRO.currentTime >= config.DamageDelayTime)
            {
                var collisionWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>().CollisionWorld;
                var hits = new NativeList<DistanceHit>(state.WorldUpdateAllocator);

                var transformLookup = SystemAPI.GetComponentLookup<LocalTransform>();

                float totalArea = config.MaxArea;

                hits.Clear();
                
                //TODO: fixa smidigare...
                foreach (var (configEntity, hitBuffer) in
                         SystemAPI.Query<RefRW<ThunderBoltConfig>, DynamicBuffer<HitBufferElement>>())
                {

                    if (collisionWorld.OverlapSphere(transform.ValueRO.Position + new float3(0, -config.VfxHeightOffset, 0), totalArea,
                            ref hits, _detectionFilter))
                    {
                        foreach (var hit in hits)
                        {
                            var enemyPos = transformLookup[hit.Entity].Position;
                            var colPos = hit.Position;
                            float3 directionToHit = math.normalizesafe((enemyPos - transform.ValueRO.Position));

                            //Maybe TODO: kolla om hit redan finns i buffer
                            HitBufferElement element = new HitBufferElement
                            {
                                IsHandled = false,
                                HitEntity = hit.Entity,
                                Position = colPos,
                                Normal = directionToHit
                            };
                            hitBuffer.Add(element);
                        }
                    }

                    ability.ValueRW.HasFired = true;
                }
            }
        }
        
        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }
}
