using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class InputAuthoring : MonoBehaviour
{
    class Baker : Baker<InputAuthoring>
    {
        public override void Bake(InputAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.None);
                
            // Move Inputs
            AddComponent<PlayerMoveInput>(entity);
            AddComponent<PlayerDashInput>(entity);
            
            // Mouse Inputs
            AddComponent<MousePositionInput>(entity);

            // Attack Inputs
            AddComponent<PlayerNormalAttackInput>(entity);
            AddComponent<PlayerSpecialAttackInput>(entity);
            AddComponent<PlayerUltimateAttackInput>(entity);
            
            // Weapon Switch Input
            AddComponent<WeaponOneInput>(entity);
            AddComponent<WeaponTwoInput>(entity);
            AddComponent<WeaponThreeInput>(entity);

            // General Input
            AddComponent<PrimaryButtonInput>(entity);
        }
    }
}


public struct PlayerMoveInput : IComponentData
{
    public float2 Value;
}

public struct MousePositionInput : IComponentData
{
    public float2 ScreenPosition;
    public float3 WorldPosition;
}

public struct PlayerNormalAttackInput : IComponentData
{
    public bool KeyDown; 
    public bool KeyUp;
    public bool IsHeld;
}

public struct PlayerDashInput : IComponentData
{
    public bool KeyDown;
    public bool KeyUp;
    public bool IsHeld;
}

public struct CanPerformNormalAttack : IComponentData
{
    public bool Value;
}

public struct PlayerSpecialAttackInput : IComponentData
{
    public bool KeyDown;
    public bool KeyUp;
    public bool IsHeld;
}

public struct PlayerUltimateAttackInput : IComponentData
{
    public bool KeyDown;
    public bool KeyUp;
    public bool IsHeld;
}

public struct WeaponOneInput : IComponentData
{
    public bool KeyPressed;
}

public struct WeaponTwoInput : IComponentData
{
    public bool KeyPressed;
}

public struct WeaponThreeInput : IComponentData
{
    public bool KeyPressed;
}