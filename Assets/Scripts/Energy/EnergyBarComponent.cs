using Unity.Entities;

public struct EnergyBarComponent : IComponentData
{
    public float CurrentEnergy;
    public float MaxEnergy;
}