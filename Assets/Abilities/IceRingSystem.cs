using System.Collections;
using System.Collections.Generic;
using Player;
using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

[BurstCompile]
public partial struct IceRingSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<IceRingAbility>();
        state.RequireForUpdate<IceRingConfig>();
        state.RequireForUpdate<PlayerPositionSingleton>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var config = SystemAPI.GetSingleton<IceRingConfig>();

        foreach (var (ability, timer, transform, entity) in
                 SystemAPI.Query<RefRW<ThunderStrikeAbility>, RefRW<TimerObject>, RefRW<LocalTransform>>()
                     .WithEntityAccess())
        {
            Debug.Log($"{ability.ValueRO.test}");

            timer.ValueRW.currentTime += SystemAPI.Time.DeltaTime;
            if(timer.ValueRO.currentTime > timer.ValueRO.maxTime) Debug.Log("gluffs");
        }
    }
}
