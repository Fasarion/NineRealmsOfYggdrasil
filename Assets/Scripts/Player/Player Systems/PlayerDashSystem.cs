﻿using Movement;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;

namespace Player
{
    public partial struct PlayerDashSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<PlayerDashInput>();
            state.RequireForUpdate<PlayerDashConfig>();
            state.RequireForUpdate<PlayerTag>(); 
            state.RequireForUpdate<PlayerPositionSingleton>(); 
            state.RequireForUpdate<PlayerMoveInput>();
            state.RequireForUpdate<WeaponAttackCaller>();
        }
    
        public void OnUpdate(ref SystemState state)
        {
            var dashInput = SystemAPI.GetSingleton<PlayerDashInput>();
            var dashConfig = SystemAPI.GetSingletonRW<PlayerDashConfig>();
            
            var dashTimer = SystemAPI.GetComponentRW<TimerObject>(SystemAPI.GetSingletonEntity<PlayerDashConfig>());
            var attackCaller = SystemAPI.GetSingleton<WeaponAttackCaller>();
            
            foreach (var _ in SystemAPI.Query<PlayerTag>().WithAll<CanMoveFromInput>())
            {
                // don't dash if busy
                if (attackCaller.BusyAttackInfo.Busy) continue;
                
                bool dashBufferSingletonExists =
                    SystemAPI.TryGetSingletonBuffer(out DynamicBuffer<DashInfoElement> dashBuffer);

                if (!dashBufferSingletonExists) return;

                bool playerDashInput = dashInput.KeyDown;
                bool playerCanDash = !dashConfig.ValueRO.IsDashing;

                for (int i = 0; i < dashBuffer.Length; i++)
                {
                    var dashInfo = dashBuffer.ElementAt(i);
                    dashTimer.ValueRW.currentTime += SystemAPI.Time.DeltaTime;
                    dashInfo.Value.CurrentTime += SystemAPI.Time.DeltaTime;
                    
                    if (dashInfo.Value.Ready && playerDashInput && playerCanDash)
                    {
                        EventManager.OnDashInput?.Invoke();
                        var audioBuffer = SystemAPI.GetSingletonBuffer<AudioBufferData>();
                        audioBuffer.Add(new AudioBufferData { AudioData = dashConfig.ValueRO.Audio});

                        dashInfo.Value.CurrentTime = 0f;
                        playerCanDash = false;

                        dashTimer.ValueRW.currentTime = 0f;
                    }

                    dashBuffer.ElementAt(i) = dashInfo; 
                }
            }
        }
    }
}