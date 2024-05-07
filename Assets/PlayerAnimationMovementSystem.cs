using Patrik;
using Player;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public partial struct PlayerAnimationMovementSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        // var tracker = SystemAPI.GetSingletonRW<PlayerMovementTrackerSingletonComponent>();

        if (!SystemAPI.TryGetSingleton(out PlayerMoveInput input)) return;
        if (!SystemAPI.TryGetSingletonEntity<PlayerTag>(out Entity playerEntity)) return;
        
        // TODO: make own 2D based system
        var playerForward = SystemAPI.GetComponent<LocalTransform>(playerEntity).Forward();
        
        Vector3 movement = new Vector3(input.Value.x, 0, input.Value.y);
        Vector3 lookDirection = playerForward;

        SetMovementInput(movement, lookDirection);

        // float2 rotationAsFloat2 = playerForward.xz;

        //   float rotation = Vector2.SignedAngle(input.Value, rotationAsFloat2);
        
        //  var local = input.Value * rotationAsFloat2;
      
        //  var local = rotation * input.Value;

        // tracker.ValueRW.WorldSpaceInput = input.Value;
        // tracker.ValueRW.PlayerForward = rotationAsFloat2;
        // tracker.ValueRW.LocalSpaceInput = local;
        //
        //
        // PlayerWeaponManagerBehaviour.Instance.SetMovementXY(local); 
        //PlayerWeaponManagerBehaviour.Instance.SetMovementXY(input.Value); 
    }   
    
    void SetMovementInput(Vector3 movement, Vector3 lookDirection)
    {
        float forwardBackwardsMagnitude = 0; 
        float rightLeftMagnitude = 0;
        
        if (movement.magnitude > 0) // magnitude is moar then nothing
        {
            forwardBackwardsMagnitude = Mathf.Clamp (Vector3.Dot (movement, lookDirection), -1, 1);
            Vector3 perpendicularLookingAt = new Vector3 (lookDirection.z, 0, -lookDirection.x);
            rightLeftMagnitude = Mathf.Clamp (Vector3.Dot (movement, perpendicularLookingAt), -1, 1);
        }

        PlayerWeaponManagerBehaviour.Instance.SetMovementXY(new float2(rightLeftMagnitude, forwardBackwardsMagnitude));
    }
}