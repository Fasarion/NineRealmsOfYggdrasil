using Unity.Entities;

public struct ShootPlayerWhenCloseComponent : IComponentData
{
    public float ShootingCooldownTime;
    public float CurrentCooldownTime;
}