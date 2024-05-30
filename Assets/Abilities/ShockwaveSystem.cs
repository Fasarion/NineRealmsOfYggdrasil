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

[UpdateBefore(typeof(HitStopSystem))]
[BurstCompile]
public partial struct ShockwaveSystem : ISystem
{
    private CollisionFilter _detectionFilter;
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<PlayerTargetInfoSingleton>();
        state.RequireForUpdate<PhysicsWorldSingleton>();
        state.RequireForUpdate<ShockwaveAbility>();
        state.RequireForUpdate<ThunderStrikeConfig>();
        
        _detectionFilter = new CollisionFilter
        {
            BelongsTo = 1, // Projectile
            CollidesWith = 1 << 1 | 1 << 5, // Enemy
        };
    }

    public void OnUpdate(ref SystemState state)
    {
        var thunderConfig = SystemAPI.GetSingleton<ThunderStrikeConfig>();
        var ecb = new EntityCommandBuffer(state.WorldUpdateAllocator);
        var audioBuffer = SystemAPI.GetSingletonBuffer<AudioBufferData>();
        
                foreach (var (ability, timer, transform, entity) in
                 SystemAPI.Query<RefRW<ShockwaveAbility>, RefRW<TimerObject>, RefRW<LocalTransform>>()
                     .WithEntityAccess())
        {
            timer.ValueRW.currentTime += SystemAPI.Time.DeltaTime;

            if (timer.ValueRO.currentTime > thunderConfig.maxAftermathDisplayTime)
            {
                ecb.AddComponent<ShouldBeDestroyed>(entity);
            }
            
            if (ability.ValueRO.HasFired) continue;

            if (timer.ValueRO.currentTime >= thunderConfig.damageDelayTime)
            {
                var collisionWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>().CollisionWorld;
                var hits = new NativeList<DistanceHit>(state.WorldUpdateAllocator);

                var transformLookup = SystemAPI.GetComponentLookup<LocalTransform>();

                float totalArea = thunderConfig.damageArea;

                hits.Clear();

                var hitBuffer = state.EntityManager.GetBuffer<HitBufferElement>(entity);
                

                if (collisionWorld.OverlapSphere(transform.ValueRO.Position + new float3(0, -thunderConfig.shockwaveEffectHeightOffset, 0), totalArea,
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
                        
                        var audioElement = new AudioBufferData() {AudioData = thunderConfig.impactAudioData};
                        audioBuffer.Add(audioElement);
                    }
                }

                ability.ValueRW.HasFired = true;
            }
        }
        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }
}
