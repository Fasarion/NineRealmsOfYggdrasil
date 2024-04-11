using Unity.Entities;
using Unity.Transforms;
using UnityEngine;
using Unity.Mathematics;

namespace Player
{
    public partial class PlayerRotationSystem : SystemBase
    {
        float3 previousPos = float3.zero;
        private Camera _camera;
        
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
            float rotationSpeed = 1f;
            if (SystemAPI.TryGetSingleton(out AimSettingsData aimSettings))
            {
                rotationSpeed = aimSettings.rotationSpeed;
            }
            
            
            float3 mousePosition = GetMousePosition();
            previousPos = mousePosition;
            
            foreach (var playerTransform in 
                SystemAPI.Query<RefRW<LocalTransform>>().WithAll<PlayerTag>())
            {
                var directionToMouse = mousePosition - playerTransform.ValueRO.Position;
                directionToMouse.y = 0;
                quaternion lookRotation = math.normalizesafe(quaternion.LookRotation(directionToMouse, math.up()));
                playerTransform.ValueRW.Rotation = math.slerp(playerTransform.ValueRO.Rotation, lookRotation, rotationSpeed);
            }
        }

        private float3 GetMousePosition()
        {
            // Use the Input system to get the mouse position in screen coordinates
           // Vector3 mousePosition = Input.mousePosition;
            var hasMouseInput = SystemAPI.TryGetSingleton(out MousePositionInput mousePositionInput);
            if (!hasMouseInput)
            {
                Debug.LogWarning("No mouse position found, wont rotate player.");
                return float3.zero;
            }
            
            
            float2 mousePosition = mousePositionInput.Value;

            if (!_camera)
            {
                _camera = Camera.main;
                if (!_camera)
                {
                    Debug.LogWarning("No camera found, wont rotate player.");
                    return Vector3.zero;
                }
            }
            
            Vector3 mousePosVector3 = new Vector3(mousePosition.x, mousePosition.y, 0);
            Ray ray = _camera.ScreenPointToRay(mousePosVector3);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                return hit.point;
            }

            // If the raycast doesn't hit anything, return a default position
            return previousPos;
        }
    }
     
}
