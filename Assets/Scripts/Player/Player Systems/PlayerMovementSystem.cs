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

            foreach (var (playerTransform, speedComp) 
                in SystemAPI.Query<RefRW<LocalTransform>, RefRO<MoveSpeedComponent>>().WithAll<PlayerTag>())
            {
                float speed = speedComp.ValueRO.Value;
                var newPos = playerTransform.ValueRO.Position.xz +  moveInput.Value * speed * SystemAPI.Time.DeltaTime;
                playerTransform.ValueRW.Position.xz = newPos;
                playerPosSingleton.ValueRW.Value = playerTransform.ValueRO.Position;
                
                if (PlayerWeaponManagerBehaviour.Instance != null)
                {
                    bool playerIsMoving = !moveInput.Value.Equals(float2.zero);
                    
                    // TODO: lerp
                    float t = math.length(moveInput.Value);
                    PlayerWeaponManagerBehaviour.Instance.UpdateMovementParameter(t);
                }
                
            }
        }
    }
}