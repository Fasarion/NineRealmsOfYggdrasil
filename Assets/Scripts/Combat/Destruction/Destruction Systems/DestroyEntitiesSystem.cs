using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

namespace Destruction
{
    /// <summary>
    /// System that destroys all entities with tag "ShouldBeDestroyed" enabled.
    /// </summary>
    [UpdateInGroup(typeof(SimulationSystemGroup), OrderLast = true)]
    public partial struct DestroyEntitiesSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<BeginSimulationEntityCommandBufferSystem.Singleton>();
        }
        
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var ecb = new EntityCommandBuffer(state.WorldUpdateAllocator);
            var beginSimECB = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>()
                .CreateCommandBuffer(state.WorldUnmanaged);
            
            var transformLookup = SystemAPI.GetComponentLookup<LocalTransform>();
            var spawnEntityOnDestroyLookup = SystemAPI.GetComponentLookup<SpawnEntityOnDestroy>();

            foreach (var (_, entity) in SystemAPI.Query<RefRW<ShouldBeDestroyed>>().WithEntityAccess())
            {
                
                // Hanlde
                if (spawnEntityOnDestroyLookup.TryGetComponent(entity, out var spawnEntityOnDestroy))
                {
                    var spawnedEntity = beginSimECB.Instantiate(spawnEntityOnDestroy.Value);
                    if (transformLookup.TryGetComponent(entity, out var transform))
                    {
                        var localTransform = SystemAPI.GetComponent<LocalTransform>(spawnEntityOnDestroy.Value);
                        localTransform.Position = transform.Position;
                        localTransform.Rotation = transform.Rotation;
                        beginSimECB.SetComponent(spawnedEntity, localTransform);
                    }
                }
                
                ecb.DestroyEntity(entity);
            }

            ecb.Playback(state.EntityManager);
        }
    }
}