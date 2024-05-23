using Unity.Entities;
using UnityEngine;

public class GetStatsFromOwnerAuthoring : MonoBehaviour
{
    class Baker : Baker<GetStatsFromOwnerAuthoring>
    {
        public override void Bake(GetStatsFromOwnerAuthoring fromOwnerAuthoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent<UpdateStatsComponent>(entity);
        }
    }
}

public struct UpdateStatsComponent : IComponentData
{
    public Entity EntityToTransferStatsFrom;
}