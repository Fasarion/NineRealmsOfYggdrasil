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
        playerInputActions.InputMap.PlayerSpecialAttack.performed += OnSpecialAttackDown;
        playerInputActions.InputMap.PlayerSpecialAttack.canceled += OnSpecialAttackUp;
        playerInputActions.InputMap.PlayerUltimateAttack.performed += OnUltimateAttack;
        
        // Weapon switch
        playerInputActions.WeaponMap.SwitchWeapon1.performed += OnWeapon1;
        playerInputActions.WeaponMap.SwitchWeapon2.performed += OnWeapon2;
        playerInputActions.WeaponMap.SwitchWeapon3.performed += OnWeapon3;
       
        
        // UI
        playerInputActions.InputMap.UpgradeUIButton.performed += OnUpgradeUIButtonPressed;
        
        
        playerEntity = SystemAPI.GetSingletonEntity<PlayerTag>();
    }
    


    protected override void OnUpdate()
    {
        var currentMovementActions = playerInputActions.InputMap.PlayerMovement.ReadValue<Vector2>();
        SystemAPI.SetSingleton(new PlayerMoveInput{Value = currentMovementActions});

        var currentMousePos = playerInputActions.InputMap.MousePosition.ReadValue<Vector2>();
        SystemAPI.SetSingleton(new MousePositionInput{ScreenPosition = currentMousePos});
    }

    protected override void OnStopRunning()
    {
        playerInputActions.Disable();
        
        playerInputActions.InputMap.PlayerNormalAttack.performed -= OnNormalAttack;
        playerInputActions.InputMap.PlayerSpecialAttack.performed -= OnSpecialAttackDown;
        playerInputActions.InputMap.PlayerSpecialAttack.canceled -= OnSpecialAttackUp;
        playerInputActions.InputMap.PlayerUltimateAttack.performed -= OnUltimateAttack;
        
        // Weapon switch
        playerInputActions.WeaponMap.SwitchWeapon1.performed -= OnWeapon1;
        playerInputActions.WeaponMap.SwitchWeapon2.performed -= OnWeapon2;
        playerInputActions.WeaponMap.SwitchWeapon3.performed -= OnWeapon3;

        playerInputActions.InputMap.UpgradeUIButton.performed -= OnUpgradeUIButtonPressed;
    }

    private void OnSpecialAttackUp(InputAction.CallbackContext obj)
    {
        if (!SystemAPI.Exists(playerEntity)) return;

        var fireInput = SystemAPI.GetSingletonRW<PlayerSpecialAttackInput>();
        fireInput.ValueRW.KeyUp = true;
        fireInput.ValueRW.IsHeld = false;
    }

    private void OnWeapon1(InputAction.CallbackContext obj)
    {
        if (!SystemAPI.Exists(playerEntity)) return;

        var fireInput = SystemAPI.GetSingletonRW<WeaponOneInput>();
        fireInput.ValueRW.KeyPressed = true; 
    }
    
    private void OnWeapon2(InputAction.CallbackContext obj)
    {
        if (!SystemAPI.Exists(playerEntity)) return;

        var fireInput = SystemAPI.GetSingletonRW<WeaponTwoInput>();
        fireInput.ValueRW.KeyPressed = true; 
    }

    private void OnWeapon3(InputAction.CallbackContext obj)
    {
        if (!SystemAPI.Exists(playerEntity)) return;

        var fireInput = SystemAPI.GetSingletonRW<WeaponThreeInput>();
        fireInput.ValueRW.KeyPressed = true; 
    }


    private void OnNormalAttack(InputAction.CallbackContext obj)
    {
        if (!SystemAPI.Exists(playerEntity)) return;

        var fireInput = SystemAPI.GetSingletonRW<PlayerNormalAttackInput>();
        fireInput.ValueRW.KeyPressed = true;
    }
    
    private void OnSpecialAttackDown(InputAction.CallbackContext obj)
    {
        if (!SystemAPI.Exists(playerEntity)) return;

        var fireInput = SystemAPI.GetSingletonRW<PlayerSpecialAttackInput>();
        fireInput.ValueRW.KeyDown = true;
        fireInput.ValueRW.IsHeld = true;
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