using System.Collections;
using System.Collections.Generic;
using Player;
using Unity.Entities;
using UnityEngine;
using UnityEngine.InputSystem;

[UpdateInGroup(typeof(InitializationSystemGroup), OrderLast = true)]
public partial class GetPlayerInputSystem : SystemBase
{
    private InputActions playerInputActions;
    private Entity playerEntity;
    
    protected override void OnCreate()
    {
        RequireForUpdate<PlayerTag>();
        playerInputActions = new InputActions();
    }

    protected override void OnStartRunning()
    {
        playerInputActions.Enable();
        
        // Attack
        playerInputActions.InputMap.PlayerNormalAttack.performed += OnNormalAttack;
        playerInputActions.InputMap.PlayerSpecialAttack.performed += OnSpecialAttack;
        playerInputActions.InputMap.PlayerUltimateAttack.performed += OnUltimateAttack;
       
        
        // General
        playerInputActions.InputMap.UpgradeUIButton.performed += OnUpgradeUIButtonPressed;
        
        
        playerEntity = SystemAPI.GetSingletonEntity<PlayerTag>();
    }

    

    protected override void OnUpdate()
    {
        var currentMovementActions = playerInputActions.InputMap.PlayerMovement.ReadValue<Vector2>();
        SystemAPI.SetSingleton(new PlayerMoveInput{Value = currentMovementActions});

        var currentMousePos = playerInputActions.InputMap.MousePosition.ReadValue<Vector2>();
        SystemAPI.SetSingleton(new MousePositionInput{Value = currentMousePos});
    }

    protected override void OnStopRunning()
    {
        playerInputActions.Disable();
        
        playerInputActions.InputMap.PlayerNormalAttack.performed -= OnNormalAttack;
        playerInputActions.InputMap.PlayerSpecialAttack.performed -= OnSpecialAttack;
        playerInputActions.InputMap.PlayerUltimateAttack.performed -= OnUltimateAttack;

        playerInputActions.InputMap.UpgradeUIButton.performed -= OnUpgradeUIButtonPressed;
    }

    

    private void OnNormalAttack(InputAction.CallbackContext obj)
    {
        if (!SystemAPI.Exists(playerEntity)) return;

        var fireInput = SystemAPI.GetSingletonRW<PlayerNormalAttackInput>();
        fireInput.ValueRW.KeyPressed = true;
    }
    
    private void OnSpecialAttack(InputAction.CallbackContext obj)
    {
        if (!SystemAPI.Exists(playerEntity)) return;

        var fireInput = SystemAPI.GetSingletonRW<PlayerSpecialAttackInput>();
        fireInput.ValueRW.KeyPressed = true;
    }
    
    private void OnUltimateAttack(InputAction.CallbackContext obj)
    {
        if (!SystemAPI.Exists(playerEntity)) return;

        var fireInput = SystemAPI.GetSingletonRW<PlayerUltimateAttackInput>();
        fireInput.ValueRW.KeyPressed = true;
    }

    private void OnUpgradeUIButtonPressed(InputAction.CallbackContext obj)
    {
        if (!SystemAPI.Exists(playerEntity)) return;
        
        var uiButtonInput = SystemAPI.GetSingletonRW<UpgradeUIButtonPress>();
        uiButtonInput.ValueRW.isPressed = true;
    }
}