using Unity.Entities;

public struct EnergyFillComponent : IComponentData
{
    public float PassiveFillPerHit;
    public float ActiveFillPerHit;
} 