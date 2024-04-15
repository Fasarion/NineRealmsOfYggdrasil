using Unity.Entities;
using UnityEngine;

public class PlayerUltimateAttackAuthoring : MonoBehaviour
{
    class Baker : Baker<PlayerUltimateAttackAuthoring>
    {
        public override void Bake(PlayerUltimateAttackAuthoring spawnerAuthoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);

            AddComponent(entity, new PerformUltimateAttack());
        }
    }
}

public partial struct PerformUltimateAttack : IComponentData
{
    public bool Value;
}