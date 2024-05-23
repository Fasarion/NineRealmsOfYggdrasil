using Unity.Entities;

public struct AttackPlayerWhenCloseComponent : IComponentData
{
    public float ShootingCooldownTime;
    public float CurrentCooldownTime;

    public float MinimumDistanceForShootingSquared;
}