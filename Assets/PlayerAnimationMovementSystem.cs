using System.Numerics;
using Patrik;
using Player;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public partial struct PlayerAnimationMovementSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        // var tracker = SystemAPI.GetSingletonRW<PlayerMovementTrackerSingletonComponent>();

        if (!SystemAPI.TryGetSingleton(out PlayerMoveInput input)) return;
        if (!SystemAPI.TryGetSingletonEntity<PlayerTag>(out Entity playerEntity)) return;
        
        var playerForward = SystemAPI.GetComponent<LocalTransform>(playerEntity).Forward();
        
        Vector3 movement = new Vector3(input.Value.x, 0, input.Value.y);
        Vector3 lookDirection = playerForward;

        float2 stackOverFlowMethod = GetMoveInputSO(movement, lookDirection);
        PlayerWeaponManagerBehaviour.Instance.SetMovementXY(stackOverFlowMethod);
    }

    private float2 GetMoveInputTrig(ref SystemState state, float2 moveInput, float2 playerForward)
    {
        if (math.length(moveInput) <= 0)
        {
            return float2.zero;
        }
    
        float inputAngle = math.atan2(moveInput.y, moveInput.x);
        float playerAngle = math.atan2(playerForward.y, playerForward.x);
        
        float newAngle = (inputAngle + playerAngle);
    
        float x = math.cos(newAngle);
        float y = math.sin(newAngle);
    
        return new float2(x, y);
    }
    
    private float2 GetMoveInputSpaceChange(ref SystemState state, float2 moveInput, float2 playerForward)
    {
        return new float2
        {
            x = moveInput.x * playerForward.x - moveInput.y * playerForward.y,
            y = moveInput.x * playerForward.y + moveInput.y * playerForward.x,
        };
    }

    float2 GetMoveInputSO(Vector3 movement, Vector3 lookDirection)
    {
        float forwardBackwardsMagnitude = 0; 
        float rightLeftMagnitude = 0;
        
        if (movement.magnitude > 0)
        {
            forwardBackwardsMagnitude = Mathf.Clamp (Vector3.Dot (movement, lookDirection), -1, 1);
            Vector3 perpendicularLookingAt = new Vector3 (lookDirection.z, 0, -lookDirection.x);
            rightLeftMagnitude = Mathf.Clamp (Vector3.Dot (movement, perpendicularLookingAt), -1, 1);
        }

        return new float2(rightLeftMagnitude, forwardBackwardsMagnitude);
    }
}