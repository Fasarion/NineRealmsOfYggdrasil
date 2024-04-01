using System.Collections;
using System.Collections.Generic;
using Player;
using Unity.Entities;
using UnityEngine;
using UnityEngine.InputSystem;

[UpdateInGroup(typeof(InitializationSystemGroup), OrderLast = true)]
public partial class GetPlayerInputSystem : SystemBase
{
    private MovementActions playerMovementActions;
    private Entity playerEntity;
    
    protected override void OnCreate()
    {
        RequireForUpdate<PlayerTag>();
        playerMovementActions = new MovementActions();
    }

    protected override void OnStartRunning()
    {
        playerMovementActions.Enable();
        playerMovementActions.MovementMap.PlayerFire.performed += OnPlayerShoot;
        playerEntity = SystemAPI.GetSingletonEntity<PlayerTag>();
    }

    protected override void OnUpdate()
    {
        var currentMovementActions = playerMovementActions.MovementMap.PlayerMovement.ReadValue<Vector2>();
        SystemAPI.SetSingleton(new PlayerMoveInput{Value = currentMovementActions});
    }

    protected override void OnStopRunning()
    {
        playerMovementActions.Disable();
        playerMovementActions.MovementMap.PlayerFire.performed -= OnPlayerShoot;
    }

    private void OnPlayerShoot(InputAction.CallbackContext obj)
    {
        if (!SystemAPI.Exists(playerEntity)) return;

        var fireInput = SystemAPI.GetSingletonRW<PlayerFireInput>();
        fireInput.ValueRW.FireKeyPressed = true;
    }
}