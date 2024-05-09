using Unity.Entities;

public struct MoveTowardsPlayerComponent : IComponentData
{
    public float MinimumDistanceForMovingSquared;
    public float MoveAwayFromPlayerSpeed;
}