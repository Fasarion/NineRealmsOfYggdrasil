using System;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics.Systems;
using Unity.Transforms;
using UnityEngine;
using Movement;
using Patrik;
using Unity.Physics;

namespace Player
{
    /// <summary>
    /// System for moving a player based on its move speed and sprint modifier.
    /// </summary>
    public partial struct PlayerMovementSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<PlayerDashInput>();
            state.RequireForUpdate<PlayerDashConfig>();
            state.RequireForUpdate<PlayerTag>(); 
            state.RequireForUpdate<PlayerPositionSingleton>(); 
            state.RequireForUpdate<PlayerMoveInput>();
        }
    
        public void OnUpdate(ref SystemState state)
        {
            var playerPosSingleton = SystemAPI.GetSingletonRW<PlayerPositionSingleton>();
            var moveInput = SystemAPI.GetSingleton<PlayerMoveInput>();
            var dashInput = SystemAPI.GetSingleton<PlayerDashInput>();
            var dashConfig = SystemAPI.GetSingletonRW<PlayerDashConfig>();
            var dashTimer = SystemAPI.GetComponentRW<TimerObject>(SystemAPI.GetSingletonEntity<PlayerDashConfig>());
            
            foreach (var (playerTransform, speedComp, animatorReference, gameObjectAnimator, velocity) 
                in SystemAPI.Query<RefRW<LocalTransform>, RefRO<MoveSpeedComponent>, AnimatorReference, GameObjectAnimatorPrefab, RefRW<PhysicsVelocity>>()
                    .WithAll<PlayerTag, CanMoveFromInput>())
            {
                float speed = speedComp.ValueRO.Value;

                Vector3 moveInputVec3 = new Vector3(moveInput.Value.x, 0, moveInput.Value.y);
                Vector3 step = moveInputVec3 * speed * SystemAPI.Time.DeltaTime;

                if (!gameObjectAnimator.FollowEntity)
                {
                    animatorReference.Animator.transform.position += step;
                }
                // else
                // {
                //     playerTransform.ValueRW.Position += (float3)step;
                // }
                
                playerPosSingleton.ValueRW.Value = playerTransform.ValueRO.Position;
                
                // Check for dash input - and apply dash force
                if (dashInput.KeyDown && !dashConfig.ValueRO.IsDashing && !dashConfig.ValueRO.IsDashOnCooldown)
                {
                    Debug.Log("Dash!");
                    dashTimer.ValueRW.currentTime = 0;
                    dashConfig.ValueRW.IsDashing = true;
                    dashConfig.ValueRW.IsDashOnCooldown = true;

                    velocity.ValueRW.Linear += (float3)moveInputVec3 * dashConfig.ValueRO.DashForce;
                    
                    gameObjectAnimator.FollowEntity = true;
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