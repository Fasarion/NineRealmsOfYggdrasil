using System.Collections;
using System.Collections.Generic;
using Damage;
using Health;
using Unity.Burst;
using Unity.Entities;
using UnityEngine;

[UpdateBefore(typeof(PlaySoundsSystem))]
// [UpdateAfter(typeof(AddDamageBufferElementOnTriggerSystem))]
// [UpdateBefore(typeof(DisableHasChangedHealthTagsSystem))]

[UpdateInGroup(typeof(CombatSystemGroup))]
[UpdateAfter(typeof(ApplyDamageSystem))]
[UpdateBefore(typeof(DisableHasChangedHealthTagsSystem))]

//[UpdateInGroup(typeof(CombatSystemGroup))]
public partial struct AddSoundOnDamageChangeSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
    }
    
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var soundCaller = SystemAPI.GetSingletonBuffer<AudioBufferData>();
        
        foreach (var audioDataComponent in 
            SystemAPI.Query<PlaySoundOnHitComponent>().WithAll<HasChangedHP>())
        {
            soundCaller.Add(new AudioBufferData {AudioData = audioDataComponent.Value});
        }
    }
}