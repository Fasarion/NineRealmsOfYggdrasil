using Unity.Entities;
using Unity.Mathematics;
using UnityEngine.Serialization;

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