using Unity.Burst;
using Unity.Entities;

namespace Destruction
{
    /// <summary>
    /// System that destroys all entities with tag "ShouldBeDestroyed" enabled.
    /// </summary>
    [UpdateInGroup(typeof(SimulationSystemGroup), OrderLast = true)]
    public partial struct DestroyEntitiesSystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var ecb = new EntityCommandBuffer(state.WorldUpdateAllocator);

            foreach (var (_, entity) in SystemAPI.Query<RefRW<ShouldBeDestroyed>>().WithEntityAccess())
            {
                ecb.DestroyEntity(entity);
            }

            ecb.Playback(state.EntityManager);
        }
    }
}