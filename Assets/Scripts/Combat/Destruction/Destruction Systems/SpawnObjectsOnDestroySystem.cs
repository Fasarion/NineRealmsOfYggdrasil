using Damage;
using Movement;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace Destruction
{
    [UpdateBefore(typeof(DestroyEntitiesSystem))]
    [UpdateAfter(typeof(ApplyDamageSystem))]
    public partial struct SpawnObjectsOnDestroySystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<BeginSimulationEntityCommandBufferSystem.Singleton>();
        }
        
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var ecb = new EntityCommandBuffer(Allocator.Temp);
            var beginSimECB = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>()
                .CreateCommandBuffer(state.WorldUnmanaged);
            
            var transformLookup = SystemAPI.GetComponentLookup<LocalTransform>();
            
            foreach (var (spawnObject, entity)
                in SystemAPI.Query<SpawnEntityOnDestroy>()
                .WithAll<ShouldBeDestroyed>()
                .WithEntityAccess())
            {
                Debug.Log("Spawn Entity");
                
                var spawnedEntity = beginSimECB.Instantiate(spawnObject.Value);
                if (transformLookup.TryGetComponent(entity, out var spawnTransform))
                {
                    var localTransform = SystemAPI.GetComponent<LocalTransform>(spawnObject.Value);
                    localTransform.Position = spawnTransform.Position;
                    localTransform.Rotation = spawnTransform.Rotation;
                    beginSimECB.SetComponent(spawnedEntity, localTransform);
                }

                // var xpObject = spawnObject.ValueRO.Value;
                // state.EntityManager.SetComponentData(xpObject, new LocalTransform
                // {
                //     Position = transform.Position,
                //     Rotation = quaternion.identity,
                //     Scale = 1 
                // });
            }
            
            ecb.Playback(state.EntityManager);
        }
    }
}