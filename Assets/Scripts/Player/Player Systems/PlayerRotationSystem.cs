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
            // player aim config does not exist
            if (!SystemAPI.TryGetSingleton(out PlayerRotationConfig _))
            {
                return;
            }
            
            AimSettingsData data = SystemAPI.GetSingleton<AimSettingsData>();

            if (data.autoAim)
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
            float3 mousePosition = GetMousePosition();
            previousPos = mousePosition;
            
            foreach (var playerTransform in 
                SystemAPI.Query<RefRW<LocalTransform>>().WithAll<PlayerTag>())
            {
                var directionToMouse = mousePosition - playerTransform.ValueRO.Position;
                directionToMouse.y = 0;
                quaternion lookRotation = math.normalizesafe(quaternion.LookRotation(directionToMouse, math.up()));
                playerTransform.ValueRW.Rotation = lookRotation; 
            }
        }

        private float3 GetMousePosition()
        {
            // Use the Input system to get the mouse position in screen coordinates
            Vector3 mousePosition = Input.mousePosition;

            if (!_camera) _camera = Camera.main;

            // Convert the screen coordinates to world coordinates
            Ray ray = _camera.ScreenPointToRay(mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                return hit.point;
            }

            // If the raycast doesn't hit anything, return a default position
            return previousPos;
        }
    }
     
}
