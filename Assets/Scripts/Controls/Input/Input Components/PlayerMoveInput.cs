using Unity.Entities;
using Unity.Mathematics;

public struct PlayerMoveInput : IComponentData
{
    public float2 Value;
}

public struct PlayerNormalAttackInput : IComponentData
{
    public bool KeyPressed;
}

public struct PlayerSpecialAttackInput : IComponentData
{
    public bool KeyPressed;
}

public struct PlayerUltimateAttackInput : IComponentData
{
    public bool KeyPressed;
}