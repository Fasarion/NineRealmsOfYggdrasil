using Health;
using Unity.Burst;
using Unity.Entities;

namespace Damage
{
    /// <summary>
    /// System for disabling compontent "HasChangedHealth" after all logic for handling what happens when an entity has
    /// changed their health
    /// </summary>
    [UpdateInGroup(typeof(CombatSystemGroup))]
    [UpdateAfter(typeof(ApplyDamageSystem))]
    public partial struct DisableHasChangedHealthTagsSystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            foreach (var (healthChange, entity) in SystemAPI
                .Query<RefRW<HasChangedHP>>()
                .WithEntityAccess())
            {
                // reset HP change amount
                SystemAPI.SetComponent(entity, new HasChangedHP(0));
                
                // Disable component
                SystemAPI.SetComponentEnabled<HasChangedHP>(entity, false);
            }
         
        }
    }
}