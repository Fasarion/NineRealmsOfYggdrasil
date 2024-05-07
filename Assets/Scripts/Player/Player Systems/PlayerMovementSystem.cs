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
            state.RequireForUpdate<PlayerTag>(); 
            state.RequireForUpdate<PlayerPositionSingleton>(); 
            state.RequireForUpdate<PlayerMoveInput>();
        }
    
        public void OnUpdate(ref SystemState state)
        {
            var playerPosSingleton = SystemAPI.GetSingletonRW<PlayerPositionSingleton>();
            var moveInput = SystemAPI.GetSingleton<PlayerMoveInput>();

            foreach (var (playerTransform, speedComp, animatorReference) 
                in SystemAPI.Query<RefRW<LocalTransform>, RefRO<MoveSpeedComponent>, AnimatorReference>().WithAll<PlayerTag>())
            {
                float speed = speedComp.ValueRO.Value;

                Vector3 moveInputVec3 = new Vector3(moveInput.Value.x, 0, moveInput.Value.y);
                Vector3 step = moveInputVec3 * speed * SystemAPI.Time.DeltaTime;

                animatorReference.Animator.transform.position += step;
                playerPosSingleton.ValueRW.Value = playerTransform.ValueRO.Position;
            }
        }
    }
}