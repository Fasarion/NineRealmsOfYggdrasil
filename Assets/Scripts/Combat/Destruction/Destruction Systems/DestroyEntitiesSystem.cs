using Damage;
using Player;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
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
            state.RequireForUpdate<PlayerPositionSingleton>();
            state.RequireForUpdate<BeginSimulationEntityCommandBufferSystem.Singleton>();
        }
        
        //[BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var ecb = new EntityCommandBuffer(state.WorldUpdateAllocator);
            var knockBackBufferLookup = SystemAPI.GetBufferLookup<KnockBackBufferElement>();
            var playerPos = SystemAPI.GetSingleton<PlayerPositionSingleton>();
            
            var spawnEntityOnDestroyLookup = SystemAPI.GetBufferLookup<SpawnEntityOnDestroyElement>();
            var spawnSettingsLookup = SystemAPI.GetComponentLookup<SpawnSettingsComponent>();
            
            RefRW<RandomComponent> random = SystemAPI.GetSingletonRW<RandomComponent>();

            foreach (var (dyingComponent, transform, entity) in SystemAPI.
                Query<RefRW<IsDyingComponent>, LocalTransform>()
                .WithAll<SpawnOnDestroyTag>()
                .WithEntityAccess())
            {
                if (dyingComponent.ValueRO.HasDoneSpawning) continue;

                dyingComponent.ValueRW.HasDoneSpawning = true;
                // Spawn Objects on destroy
                if (spawnEntityOnDestroyLookup.HasBuffer(entity))
                {
                    var spawnBuffer = spawnEntityOnDestroyLookup[entity];
                    var counter = 0;

                    foreach (var spawnElement in spawnBuffer)
                    {
                        var entityToSpawn = spawnElement.Value;
                        if (entityToSpawn == Entity.Null)
                        {
                            Debug.LogError($"Entity to spawn on destroy is not assigned! (from entity {entity.Index}");
                            continue;
                        }
                        
                        var spawnedEntity = state.EntityManager.Instantiate(spawnElement.Value);

                        if (state.EntityManager.HasBuffer<KnockBackBufferElement>(spawnedEntity))
                        {
                            var knockBackBufferElements = knockBackBufferLookup[spawnedEntity];
                            var forceDirection = math.normalize(transform.Position - (playerPos.Value - new float3(0, 15f, 0)));
                            var knockBackForce = 7f;

                            if (counter > 0)
                            {
                                var radAngle = random.ValueRW.random.NextFloat(0f, math.PI * 2);
                                knockBackForce = random.ValueRW.random.NextFloat(5, 10);
                                var rotationQ = quaternion.RotateY(radAngle);
                                forceDirection = math.rotate(rotationQ, forceDirection);
                            }
                            
                            knockBackBufferElements.Add(new KnockBackBufferElement
                            {
                                KnockBackForce = forceDirection * knockBackForce,
                            });
                        }

                        var spawnedTransform = state.EntityManager.GetComponentData<LocalTransform>(spawnedEntity);
                        spawnedTransform.Position = transform.Position;

                        var spawnSettings = spawnSettingsLookup.GetRefRO(entity).ValueRO.Value;
                        
                        if (spawnSettings.SetScale)
                        {
                            spawnedTransform.Scale = spawnSettings.NewScale;
                        }
                        
                        if (spawnSettings.SetYPosition) 
                        {
                            spawnedTransform.Position.y = spawnSettings.YPosition;
                        }
                        
                        spawnedTransform.Rotation = transform.Rotation;
                        state.EntityManager.SetComponentData(spawnedEntity, spawnedTransform);
                        counter++;
                    }
                }
            }

            foreach (var (_, entity) in SystemAPI.Query<ShouldBeDestroyed>()
                         .WithEntityAccess())
            {
                ecb.DestroyEntity(entity);
                
                DestroyChildrenRecursively(ref state, entity, ecb);
            }

            ecb.Playback(state.EntityManager);
            ecb.Dispose();
        }

        // TODO: make burstable
        //[BurstCompile]
        private static void DestroyChildrenRecursively(ref SystemState state, Entity entity, EntityCommandBuffer ecb)
        {
            if (state.EntityManager.HasBuffer<Child>(entity))
            {
                DynamicBuffer<Child> childBuffer = state.EntityManager.GetBuffer<Child>(entity);

                foreach (var child in childBuffer)
                {
                    DestroyChildrenRecursively(ref state, child.Value, ecb);
                    ecb.DestroyEntity(child.Value);
                }
            }
        }
    }
}