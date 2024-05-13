using System.Collections;
using System.Collections.Generic;
using Damage;
using Movement;
using Unity.Burst;
using Unity.Entities;
using UnityEngine;

[BurstCompile]
[UpdateBefore(typeof(HandleHitBufferSystem))]
[UpdateInGroup(typeof(CombatSystemGroup))]
public partial struct HitStopSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var ecb = new EntityCommandBuffer(state.WorldUpdateAllocator);
        
        //Apply Hitstop
        foreach (var (hitBuffer, applyHitStop)
                 in SystemAPI.Query<DynamicBuffer<HitBufferElement>, ShouldApplyHitStopOnHit>())
        {
            foreach (var hit in hitBuffer)
            {
                if (hit.IsHandled) continue;
                if (!state.EntityManager.HasComponent<HitStopComponent>(hit.HitEntity))
                {
                    ecb.AddComponent<HitStopComponent>(hit.HitEntity);

                    ecb.SetComponent(hit.HitEntity, new HitStopComponent
                    {
                        MaxDuration = applyHitStop.Duration,
                    });
                }
                
                
            }
        }

        foreach (var (hitStop, entity) in SystemAPI.Query<RefRW<HitStopComponent>>().WithAll<MoveSpeedComponent>().WithEntityAccess())
        {
            if (hitStop.ValueRO.CurrentElapsedTime > hitStop.ValueRO.MaxDuration)
            {
                ecb.RemoveComponent<HitStopComponent>(entity);
            }
            else
            {
                hitStop.ValueRW.CurrentElapsedTime += SystemAPI.Time.DeltaTime;
            }
        }
        
        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }
}
