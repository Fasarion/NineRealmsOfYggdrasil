using Unity.Entities;
using Unity.Mathematics;

public struct PlayerMoveInput : IComponentData
{
    public float2 Value;
}

public struct PlayerFireInput : IComponentData
{
    public bool FireKeyPressed;
}

public struct PlayerSpecialAttackInput : IComponentData
{
    public bool FireKeyPressed;
}