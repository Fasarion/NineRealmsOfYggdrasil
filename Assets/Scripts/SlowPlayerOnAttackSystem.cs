using System.Collections;
using System.Collections.Generic;
using Movement;
using Patrik;
using Player;
using Unity.Burst;
using Unity.Entities;
using UnityEngine;

[BurstCompile]
public partial struct SlowPlayerOnAttackSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<PlayerTag>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var attackCaller = SystemAPI.GetSingletonRW<WeaponAttackCaller>();

        foreach (var slowComponent in
                 SystemAPI.Query<RefRW<ShouldSlowPlayerMovementOnAttackComponent>>())
        {
            if (!slowComponent.ValueRO.IsInitialized)
            {
                var playerEntity = SystemAPI.GetSingletonEntity<PlayerTag>();

                var moveSpeedComponent = state.EntityManager.GetComponentData<MoveSpeedComponent>(playerEntity);

                slowComponent.ValueRW.CachedSpeed = moveSpeedComponent.Value;
                slowComponent.ValueRW.SlowTimer = 0;
                slowComponent.ValueRW.IsSlowing = false;
                slowComponent.ValueRW.IsInitialized = true;
            }
            
            var weaponType = slowComponent.ValueRO.WeaponType;
            if (attackCaller.ValueRO.ShouldStartActiveAttack(weaponType, AttackType.Normal))
            {
                var playerEntity = SystemAPI.GetSingletonEntity<PlayerTag>();

                state.EntityManager.SetComponentData(playerEntity, new MoveSpeedComponent
                {
                    Value = slowComponent.ValueRO.CachedSpeed * slowComponent.ValueRO.SlowPercentage,
                });

                slowComponent.ValueRW.SlowTimer = 0;
                slowComponent.ValueRW.IsSlowing = true;
            }

            if (!slowComponent.ValueRO.IsSlowing) continue;

            slowComponent.ValueRW.SlowTimer += SystemAPI.Time.DeltaTime;

            if (slowComponent.ValueRO.SlowTimer > slowComponent.ValueRO.SlowDuration)
            {
                var playerEntity = SystemAPI.GetSingletonEntity<PlayerTag>();

                state.EntityManager.SetComponentData(playerEntity, new MoveSpeedComponent
                {
                    Value = slowComponent.ValueRO.CachedSpeed,
                });

                slowComponent.ValueRW.SlowTimer = 0;
                slowComponent.ValueRW.IsSlowing = false;
            }
        }
        
        

        
            return;
    }
}
