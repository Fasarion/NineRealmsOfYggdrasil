using System.Collections;
using System.Collections.Generic;
using Damage;
using Health;
using Unity.Burst;
using Unity.Entities;
using UnityEngine;

[UpdateBefore(typeof(PlaySoundsSystem))]

[UpdateInGroup(typeof(CombatSystemGroup))]
[UpdateAfter(typeof(ApplyDamageSystem))]
[UpdateBefore(typeof(DisableHasChangedHealthTagsSystem))]
public partial struct AddSoundOnHitSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<AudioBufferData>();
    }
    
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var soundCaller = SystemAPI.GetSingletonBuffer<AudioBufferData>();
        
        // Sound on being hit
        foreach (var audioDataComponent in SystemAPI
                .Query<PlaySoundOnBeingHitComponent>()
                .WithAll<HasChangedHP>())
        {
            soundCaller.Add(new AudioBufferData {AudioData = audioDataComponent.Value});
        }
        
        // sound on hitting
        foreach (var (hitBuffer, soundOnHitting) in SystemAPI
            .Query<DynamicBuffer<HitBufferElement>, PlaySoundOnHittingComponent>())
        {
            // check for hit
            bool hasHit = false;
            foreach (var hit in hitBuffer)
            {
                if (!hit.IsHandled)
                {
                    hasHit = true;
                    break;
                }
            }
            
            if (!hasHit)
                continue;
            
            // add sound on hit
            soundCaller.Add(new AudioBufferData {AudioData = soundOnHitting.Value});
        }
    }
}

