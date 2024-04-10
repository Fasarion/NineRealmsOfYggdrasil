using Unity.Burst;
using Unity.Entities;
using Unity.Physics.Systems;

namespace Damage
{
    /// <summary>
    /// System for handling hit buffers. Marks the flag "IsHandled" in Hit Buffer Elements as True, which informs other
    /// systems to not handle the element further, i.e. registering multiple hits from the same entity.
    /// </summary>
    [UpdateInGroup(typeof(CombatSystemGroup), OrderLast = true)]
    public partial struct HandleHitBufferSystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            state.Dependency.Complete();
            
            var hitBufferLookup = SystemAPI.GetBufferLookup<HitBufferElement>();
            var triggerEntities = SystemAPI.QueryBuilder().WithAll<HitBufferElement>().Build().ToEntityArray(state.WorldUpdateAllocator);
            
            foreach (var triggerEntity in triggerEntities)
            {
                var hitBuffer = hitBufferLookup[triggerEntity];
                for (var i = 0; i < hitBuffer.Length; i++)
                {
                    hitBuffer.ElementAt(i).IsHandled = true;
                }
            }
        }
    }
}