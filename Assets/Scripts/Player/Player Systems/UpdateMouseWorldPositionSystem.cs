using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public partial class UpdateMouseWorldPositionSystem : SystemBase
{
    private Camera _camera;
    
    protected override void OnUpdate()
    {
        var hasMouseInput = SystemAPI.TryGetSingleton(out MousePositionInput mousePositionInput);
        if (!hasMouseInput)
        {
            Debug.LogWarning("No mouse position found, wont rotate player.");
            return;
        }
        
        float2 mousePositionInScreenSpace = mousePositionInput.ScreenPosition;

        if (!_camera)
        {
            _camera = Camera.main;
            if (!_camera)
            {
                Debug.LogWarning("No camera found, wont rotate player.");
                return;
            }
        }

        // update mouse world pos
        Vector3 screenPosVector3 = new Vector3(mousePositionInScreenSpace.x, mousePositionInScreenSpace.y, 0);
        Ray ray = _camera.ScreenPointToRay(screenPosVector3);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            var mouseRW = SystemAPI.GetSingletonRW<MousePositionInput>();
            mouseRW.ValueRW.WorldPosition = hit.point;

            // update mouse position entity
            foreach (var transform in SystemAPI
                .Query<RefRW<LocalTransform>>()
                .WithAll<MousePositionComponent>())
            {
                transform.ValueRW.Position = hit.point;
            }
        }
    }
}