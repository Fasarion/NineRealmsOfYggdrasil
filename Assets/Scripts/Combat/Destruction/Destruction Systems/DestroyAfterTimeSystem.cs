using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

namespace Destruction
{
    [UpdateBefore(typeof(DestroyEntitiesSystem))]
    public partial struct DestroyAfterTimeSystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var ecb = new EntityCommandBuffer(state.WorldUpdateAllocator);
        
            foreach (var (destroyObject, entity) in SystemAPI.Query<RefRW<DestroyAfterSecondsComponent>>().WithEntityAccess())
            {
                // Update current life time of entity to destroy
                destroyObject.ValueRW.CurrentLifeTime += SystemAPI.Time.DeltaTime;
            
                // When entity is at end of life time, mark it as "Should Be Destroyed". This will tell a future system to
                // destroy it
                if (destroyObject.ValueRO.CurrentLifeTime > destroyObject.ValueRO.TimeToDestroy)
                {
                    ecb.SetComponentEnabled<ShouldBeDestroyed>(entity, true);
                }
            }
        
            ecb.Playback(state.EntityManager);
        }
    }
}

