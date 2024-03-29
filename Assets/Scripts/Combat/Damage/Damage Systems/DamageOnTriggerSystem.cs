using Health;
using Unity.Burst;
using Unity.Entities;
using Unity.Physics.Systems;

namespace Damage
{
    [UpdateInGroup(typeof(PhysicsSystemGroup))]
    [UpdateBefore(typeof(HandleHitBufferSystem))]
    public partial struct DamageOnTriggerSystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var damageBufferLookup = SystemAPI.GetBufferLookup<DamageBufferElement>();
            
            foreach (var (hitBuffer, damageOnTrigger) in SystemAPI.Query<DynamicBuffer<HitBufferElement>, DamageOnTriggerComponent>())
            {
                foreach (var hit in hitBuffer)
                {
                    if (hit.IsHandled) continue;
                    var damageBuffer = damageBufferLookup[hit.HitEntity];
                    damageBuffer.Add(new DamageBufferElement
                    {
                        HitPoints = damageOnTrigger.DamageValue,
                        DamageType = damageOnTrigger.DamageType,
                    });
                }
            }
        }
    }
}