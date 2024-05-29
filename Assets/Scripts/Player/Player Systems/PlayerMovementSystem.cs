using System;
using Unity.Entities;
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
            state.RequireForUpdate<WeaponAttackCaller>();
        }
    
        public void OnUpdate(ref SystemState state)
        {
            var playerPosSingleton = SystemAPI.GetSingletonRW<PlayerPositionSingleton>();
            var moveInput = SystemAPI.GetSingleton<PlayerMoveInput>();
            
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

                //playerPosSingleton.ValueRW.Value = playerTransform.ValueRO.Position;
            }
            
            foreach (var playerTransform
                in SystemAPI.Query<RefRW<LocalTransform>>()
                    .WithAll<PlayerTag>())
            {
                playerPosSingleton.ValueRW.Value = playerTransform.ValueRO.Position;
            }
        }
    }
}