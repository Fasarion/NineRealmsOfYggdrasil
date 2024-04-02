using Unity.Entities;

namespace Health
{
    public struct ChangeHpOnImpact : IComponentData
    {
        public float Value;
    }
}