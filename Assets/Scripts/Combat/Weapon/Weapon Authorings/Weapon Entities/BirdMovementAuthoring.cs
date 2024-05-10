using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class BirdMovementAuthoring : MonoBehaviour
{
    class Baker : Baker<BirdMovementAuthoring>
    {
        public override void Bake(BirdMovementAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);

            AddComponent(entity, new BirdMovementComponent());
        }
    }
}

public struct BirdMovementComponent : IComponentData
{
    public float CurrentTValue;
    public float3 initialDirection;

    public float2 startPoint;
    public float2 controlPoint1;
    public float2 controlPoint2;
    public float2 endPoint;
}