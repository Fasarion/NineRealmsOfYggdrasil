using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;
using Unity.Mathematics;

namespace Player
{
    [UpdateAfter(typeof(UpdateMouseWorldPositionSystem))]
    public partial struct PlayerRotationSystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            bool hasAimSettings = SystemAPI.TryGetSingleton(out AimSettingsData aimSettings);

            if (hasAimSettings && aimSettings.autoAim)
            {
                HandleAutoAim(ref state);
            }
            else
            {
                HandleManualAim(ref state);
            }
        }

        [BurstCompile]
        private void HandleAutoAim(ref SystemState state)
        {
            // TODO: check if player is firing. otherwise, return
            
            // TODO: find nearest enemy, rotate to that enemy

            // TODO: Find Closest "Target" System, i.e. enemy or breakable object in range

        }

        [BurstCompile]
        private void HandleManualAim(ref SystemState state)
        {
            var hasMouseInput = SystemAPI.TryGetSingleton(out MousePositionInput mousePositionInput);
            if (!hasMouseInput)
            {
                Debug.LogWarning("No mouse position found, wont rotate player.");
                return;
            }

            if (!SystemAPI.TryGetSingletonRW(out RefRW<PlayerRotationSingleton> playerRotationSingleton))
                return;

            if (Time.timeScale <= 0) return;

            
            float rotationSpeed = 1f;
            bool slerp = false;
            if (SystemAPI.TryGetSingleton(out AimSettingsData aimSettings))
            {
                rotationSpeed = aimSettings.rotationSpeed * SystemAPI.Time.DeltaTime;
                slerp = aimSettings.slerpRotation;
            }
            
            
            float3 mousePosition = mousePositionInput.WorldPosition;

            foreach (var (playerTransform, animReference, animObject) in
                SystemAPI.Query<RefRW<LocalTransform>, AnimatorReference, GameObjectAnimatorPrefab>().WithAll<PlayerTag>())
            {
                var directionToMouse = mousePosition - playerTransform.ValueRO.Position;
                directionToMouse.y = 0;
                quaternion lookRotation = math.normalizesafe(quaternion.LookRotation(directionToMouse, math.up()));

                var newRotation = slerp
                    ? math.slerp(playerTransform.ValueRO.Rotation, lookRotation, rotationSpeed)
                    : lookRotation;
                
                if (animObject.FollowEntity)
                {
                    playerTransform.ValueRW.Rotation = newRotation;
                }
                else
                {
                    animReference.Animator.transform.rotation = newRotation;
                }

                playerRotationSingleton.ValueRW.Value = playerTransform.ValueRO.Rotation;
            }
        }
    }
     
}