using Unity.Entities;
using Unity.Physics.Systems;

namespace Damage
{
    [UpdateInGroup(typeof(PhysicsSystemGroup))]
    [UpdateAfter(typeof(DetectHpTriggerSystem))]
    public partial struct HandleHpHitBufferSystem : ISystem
    {
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