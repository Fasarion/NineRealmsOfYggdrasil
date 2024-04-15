using Unity.Entities;
using Unity.Transforms;
using UnityEngine;
using Unity.Mathematics;

namespace Player
{
    public partial class PlayerRotationSystem : SystemBase
    {
        // float3 previousPos = float3.zero;
        // private Camera _camera;
        
        protected override void OnUpdate()
        {
            bool hasAimSettings = SystemAPI.TryGetSingleton(out AimSettingsData aimSettings);

            if (hasAimSettings && aimSettings.autoAim)
            {
                HandleAutoAim();
            }
            else
            {
                HandleManualAim();
            }
        }

        private void HandleAutoAim()
        {
            // TODO: check if player is firing. otherwise, return
            
            // TODO: find nearest enemy, rotate to that enemy

            // TODO: Find Closest "Target" System, i.e. enemy or breakable object in range

        }

        private void HandleManualAim()
        {
            var hasMouseInput = SystemAPI.TryGetSingleton(out MousePositionInput mousePositionInput);
            if (!hasMouseInput)
            {
                Debug.LogWarning("No mouse position found, wont rotate player.");
                return;
            }
            
            float rotationSpeed = 1f;
            if (SystemAPI.TryGetSingleton(out AimSettingsData aimSettings))
            {
                rotationSpeed = aimSettings.rotationSpeed;
            }


            float3 mousePosition = mousePositionInput.WorldPosition;
            
            foreach (var playerTransform in 
                SystemAPI.Query<RefRW<LocalTransform>>().WithAll<PlayerTag>())
            {
                var directionToMouse = mousePosition - playerTransform.ValueRO.Position;
                directionToMouse.y = 0;
                quaternion lookRotation = math.normalizesafe(quaternion.LookRotation(directionToMouse, math.up()));
                playerTransform.ValueRW.Rotation = math.slerp(playerTransform.ValueRO.Rotation, lookRotation, rotationSpeed);
            }
        }
    }
     
}