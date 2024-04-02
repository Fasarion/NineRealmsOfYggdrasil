using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

[UpdateInGroup(typeof(TransformSystemGroup))]
public partial struct FollowParentSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        foreach (var (follow, transform, ltw, entity) in 
            SystemAPI.Query<FollowParentComponent, RefRW<LocalTransform>, RefRW<LocalToWorld>>()
                .WithEntityAccess())
        {
            var parentLTW = SystemAPI.GetComponent<LocalToWorld>(follow.ParentEntity);

            transform.ValueRW.Position = parentLTW.Position;
            transform.ValueRW.Rotation = parentLTW.Rotation;
        }
    }
}