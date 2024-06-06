using Movement;
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
            state.RequireForUpdate<GameUnpaused>();
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

                bool fillOneDashAtTime = dashConfig.ValueRO.waitBetweenDashes;
                bool oneDashIsFilled = false;

                float deltaTime = SystemAPI.Time.DeltaTime;

                SortBuffer(ref state, ref dashBuffer);

                for (int i = 0; i < dashBuffer.Length; i++)
                {
                    int index = i;

                    // if (fillOneDashAtTime)
                    // {
                    //     index = dashBuffer.Length - i - 1;
                    // }

                    var dashInfo = dashBuffer.ElementAt(index);
                    dashTimer.ValueRW.currentTime += deltaTime;

                    bool updateTimer = true;

                    if (!dashInfo.Value.Ready)
                    {
                        if (fillOneDashAtTime && oneDashIsFilled)
                            updateTimer = false;
                        
                        if (updateTimer)
                        {
                            dashInfo.Value.CurrentTime += deltaTime;
                            oneDashIsFilled = true;
                        }
                    }
                    
                    if (dashInfo.Value.Ready && playerDashInput && playerCanDash)
                    {
                        EventManager.OnDashInput?.Invoke();
                        var audioBuffer = SystemAPI.GetSingletonBuffer<AudioBufferData>();
                        audioBuffer.Add(new AudioBufferData { AudioData = dashConfig.ValueRO.Audio});

                        dashInfo.Value.CurrentTime = 0f;
                        playerCanDash = false;

                        dashTimer.ValueRW.currentTime = 0f;
                    }

                    dashBuffer.ElementAt(index) = dashInfo;
                }
            }
        }

        private void SortBuffer(ref SystemState state, ref DynamicBuffer<DashInfoElement> dashBuffer)
        {
            for (int i = 0; i < dashBuffer.Length; i++)
            {
                for (int j = i + 1; j < dashBuffer.Length; j++)
                {
                    if (dashBuffer.ElementAt(i).Value.CurrentTime <= dashBuffer.ElementAt(j).Value.CurrentTime)
                    {
                        var temp = dashBuffer.ElementAt(j);
                        dashBuffer.ElementAt(j) = dashBuffer.ElementAt(i);
                        dashBuffer.ElementAt(i) = temp;
                    }
                }
            }
        }
    }
}