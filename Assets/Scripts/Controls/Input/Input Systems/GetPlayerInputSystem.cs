using System.Collections;
using System.Collections.Generic;
using Player;
using Unity.Entities;
using UnityEngine;

[UpdateInGroup(typeof(InitializationSystemGroup), OrderLast = true)]
public partial class GetPlayerInputSystem : SystemBase
{
    private MovementActions playerMovementActions;
    
    protected override void OnCreate()
    {
        RequireForUpdate<PlayerTag>();
        playerMovementActions = new MovementActions();
    }

    protected override void OnStartRunning()
    {
        playerMovementActions.Enable();
    }

    protected override void OnUpdate()
    {
        var currentMovementActions = playerMovementActions.MovementMap.PlayerMovement.ReadValue<Vector2>();
        SystemAPI.SetSingleton(new PlayerMoveInput{Value = currentMovementActions});
    }

    protected override void OnStopRunning()
    {
        playerMovementActions.Disable();
    }
}