using Unity.Entities;

public struct FollowParentComponent : IComponentData
{
    public Entity ParentEntity;
}