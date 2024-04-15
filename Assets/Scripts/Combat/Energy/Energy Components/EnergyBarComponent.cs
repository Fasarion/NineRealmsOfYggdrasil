using Unity.Entities;

public struct EnergyBarComponent : IComponentData
{
    public float CurrentEnergy;
    public float MaxEnergy;

    public bool IsFull => CurrentEnergy >= MaxEnergy;
}