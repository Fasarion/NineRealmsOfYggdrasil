using Damage;
using Unity.Burst;
using Unity.Entities;

public partial struct SetSoundOnDamageSystem : ISystem
{
    // [BurstCompile]
    // public void OnUpdate(ref SystemState state)
    // {
    //     var soundOnHitLookup = SystemAPI.GetComponentLookup<PlaySoundOnHitComponent>();
    //         
    //     foreach (var (hitBuffer, damageOnTrigger) in SystemAPI.
    //         Query<DynamicBuffer<HitBufferElement>, DamageOnTriggerComponent>())
    //     {
    //         foreach (var hit in hitBuffer)
    //         {
    //             if (hit.IsHandled) continue;
    //     
    //             var hitEntity = hit.HitEntity;
    //     
    //             if (soundOnHitLookup.HasComponent(hitEntity))
    //             {
    //                 var playSoundComponent = soundOnHitLookup.GetRefRW(hitEntity);
    //                 playSoundComponent.ValueRW.Value.
    //             }
    //             
    //             
    //             
    //         }
    //         
    //         
    //         
    //     }
    // }
}