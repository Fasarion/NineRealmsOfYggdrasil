using Movement;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;

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
            
            foreach (var (playerTransform, speedComp, animatorReference, gameObjectAnimator, velocity) 
                in SystemAPI.Query<RefRW<LocalTransform>, RefRO<MoveSpeedComponent>, AnimatorReference, GameObjectAnimatorPrefab, RefRW<PhysicsVelocity>>()
                    .WithAll<PlayerTag, CanMoveFromInput>())
            {
                // don't dash if busy
                if (attackCaller.BusyAttackInfo.Busy) continue;
                
                // Check for dash input - and apply dash force
                if (dashInput.KeyDown && !dashConfig.ValueRO.IsDashing && !dashConfig.ValueRO.IsDashOnCooldown)
                {
                    EventManager.OnDashInput?.Invoke();
                    
                    dashTimer.ValueRW.currentTime = 0;
                    dashConfig.ValueRW.IsDashing = true;
                    dashConfig.ValueRW.IsDashOnCooldown = true;
                    
                    var audioBuffer = SystemAPI.GetSingletonBuffer<AudioBufferData>();
                    audioBuffer.Add(new AudioBufferData { AudioData = dashConfig.ValueRO.Audio});
                }
                
                dashTimer.ValueRW.currentTime += SystemAPI.Time.DeltaTime;

                if (dashConfig.ValueRO.IsDashing)
                {
                    //Check if dash is done
                    if (dashTimer.ValueRO.currentTime >= dashConfig.ValueRO.DashDuration)
                    {
                        dashConfig.ValueRW.IsDashing = false;
                        gameObjectAnimator.FollowEntity = false;
                        velocity.ValueRW.Linear = new float3(0, 0, 0);
                        
                    }
                }

                if (dashTimer.ValueRO.currentTime >= dashConfig.ValueRO.DashCooldown)
                {
                    dashConfig.ValueRW.IsDashOnCooldown = false;
                }
            }
        }
    }
}