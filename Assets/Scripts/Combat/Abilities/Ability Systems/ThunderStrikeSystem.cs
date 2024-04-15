using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Entities;
using UnityEngine;

[BurstCompile]
public partial struct ThunderStrikeSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<ThunderStrikeAbility>();
        state.RequireForUpdate<ThunderStrikeConfig>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var config = SystemAPI.GetSingleton<ThunderStrikeConfig>();

        foreach (var (ability, timer, entity) in
                 SystemAPI.Query<RefRW<ThunderStrikeAbility>, RefRW<TimerObject>>()
                     .WithEntityAccess())
        {
            ability.ValueRW.test = config.test;
            Debug.Log($"{ability.ValueRO.test}");

            timer.ValueRW.currentTime += SystemAPI.Time.DeltaTime;
            if(timer.ValueRO.currentTime > timer.ValueRO.maxTime) Debug.Log("gluffs");
        }
    }
}
