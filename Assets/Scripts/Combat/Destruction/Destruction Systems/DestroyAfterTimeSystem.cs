using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

namespace Destruction
{
    public partial struct DestroyAfterTimeSystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var ecb = new EntityCommandBuffer(state.WorldUpdateAllocator);
        
            foreach (var (destroyObject, entity) in SystemAPI.Query<RefRW<DestroyAfterSecondsComponent>>().WithEntityAccess())
            {
                destroyObject.ValueRW.CurrentLifeTime += SystemAPI.Time.DeltaTime;
            
                if (destroyObject.ValueRO.CurrentLifeTime > destroyObject.ValueRO.TimeToDestroy)
                {
                    ecb.DestroyEntity(entity);
                }
            }
        
            ecb.Playback(state.EntityManager);
        }
    }

}

