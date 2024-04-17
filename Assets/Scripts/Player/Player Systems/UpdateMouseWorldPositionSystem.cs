using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public partial class UpdateMouseWorldPositionSystem : SystemBase
{
    private Camera _camera;
    
    protected override void OnUpdate()
    {
        // Use the Input system to get the mouse position in screen coordinates
        var hasMouseInput = SystemAPI.TryGetSingleton(out MousePositionInput mousePositionInput);
        if (!hasMouseInput)
        {
            Debug.LogWarning("No mouse position found, wont rotate player.");
            return;
        }
        
        float2 mousePosition = mousePositionInput.ScreenPosition;

        if (!_camera)
        {
            _camera = Camera.main;
            if (!_camera)
            {
                Debug.LogWarning("No camera found, wont rotate player.");
                return;
            }
        }
            
        Vector3 screenPosVector3 = new Vector3(mousePosition.x, mousePosition.y, 0);
        Ray ray = _camera.ScreenPointToRay(screenPosVector3);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            var mouseRW = SystemAPI.GetSingletonRW<MousePositionInput>();
            mouseRW.ValueRW.WorldPosition = hit.point;
        }
    }
}