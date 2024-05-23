using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

namespace Destruction
{
    /// <summary>
    /// System that destroys all entities with tag "ShouldBeDestroyed" enabled.
    /// </summary>
    [UpdateInGroup(typeof(SimulationSystemGroup), OrderLast = true)]
    public partial struct DestroyEntitiesSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<BeginSimulationEntityCommandBufferSystem.Singleton>();
        }
        
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var ecb = new EntityCommandBuffer(state.WorldUpdateAllocator);
            // var beginSimECB = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>()
            //     .CreateCommandBuffer(state.WorldUnmanaged);
            //
            // var transformLookup = SystemAPI.GetComponentLookup<LocalTransform>();
            var spawnEntityOnDestroyLookup = SystemAPI.GetBufferLookup<SpawnEntityOnDestroyElement>();

            foreach (var (_, transform, entity) in SystemAPI.
                Query<RefRW<ShouldBeDestroyed>, LocalTransform>()
                .WithEntityAccess())
            {
                
                // Spawn Objects on destroy
                if (spawnEntityOnDestroyLookup.HasBuffer(entity))
                {
                    var spawnBuffer = spawnEntityOnDestroyLookup[entity];

                    foreach (var spawnElement in spawnBuffer)
                    {
                        var entityToSpawn = spawnElement.Entity;
                        if (entityToSpawn == Entity.Null)
                        {
                            Debug.LogError("Entity to spawn on destroy is not assigned!");
                            continue;
                        }
                        
                        var spawnedEntity = state.EntityManager.Instantiate(spawnElement.Entity);

                        var spawnedTransform = state.EntityManager.GetComponentData<LocalTransform>(spawnedEntity);
                        spawnedTransform.Position = transform.Position;

                        if (spawnElement.Settings.SetScale)
                        {
                            spawnedTransform.Scale = spawnElement.Settings.NewScale;
                        }
                        
                        if (spawnElement.Settings.SetYPosition)
                        {
                            spawnedTransform.Position.y = spawnElement.Settings.YPosition;
                        }
                        
                        spawnedTransform.Rotation = transform.Rotation;
                        state.EntityManager.SetComponentData(spawnedEntity, spawnedTransform);
                        
                        // TODO (NOTE): below is old code for instantiating objects on destroy, but it gave incorrect scale
                        // var spawnedEntity = beginSimECB.Instantiate(spawnElement.Value);
                        // if (transformLookup.TryGetComponent(entity, out var transform))
                        // {
                        //     var localTransform = SystemAPI.GetComponent<LocalTransform>(entity);
                        //     localTransform.Position = transform.Position;
                        //     localTransform.Rotation = transform.Rotation;
                        //     beginSimECB.SetComponent(spawnedEntity, localTransform);
                        // }
                    }
                }

                // // Destroy all children
                // DestroyChildrenRecursively(state, entity, ecb);
                
                ecb.DestroyEntity(entity);
            }

            ecb.Playback(state.EntityManager);
        }

        // TODO: make burstable
        private static void DestroyChildrenRecursively(SystemState state, Entity entity, EntityCommandBuffer ecb)
        {
            if (state.EntityManager.HasBuffer<Child>(entity))
            {
                DynamicBuffer<Child> childBuffer = state.EntityManager.GetBuffer<Child>(entity);

                foreach (var child in childBuffer)
                {
                    DestroyChildrenRecursively(state, child.Value, ecb);
                    ecb.DestroyEntity(child.Value);
                }
            }
        }
    }
}