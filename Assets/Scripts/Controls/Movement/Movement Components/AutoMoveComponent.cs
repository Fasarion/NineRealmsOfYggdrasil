using Unity.Entities;

namespace Movement
{
    public struct AutoMoveComponent : IComponentData
    {
        public bool MoveForward;
    }
}