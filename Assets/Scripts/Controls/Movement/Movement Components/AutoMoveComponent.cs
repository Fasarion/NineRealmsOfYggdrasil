using Unity.Entities;

namespace Movement
{
    public struct AutoMoveComponent : IComponentData, IEnableableComponent
    {
        public bool MoveForward;
        public float rotationSpeed;
    }
}