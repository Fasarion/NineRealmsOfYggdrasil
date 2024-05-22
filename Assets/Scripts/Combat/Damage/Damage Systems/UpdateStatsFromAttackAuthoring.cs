using Patrik;
using Unity.Entities;
using UnityEngine;

public class UpdateStatsFromAttackAuthoring : MonoBehaviour
{
    public WeaponType WeaponType;
    public AttackType AttackType;
    
    class Baker : Baker<UpdateStatsFromAttackAuthoring>
    {
        public override void Bake(UpdateStatsFromAttackAuthoring fromOwnerAuthoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new UpdateStatsFromAttack
            {
                WeaponType = fromOwnerAuthoring.WeaponType,
                AttackType = fromOwnerAuthoring.AttackType,
            });
        }
    }
}

public struct UpdateStatsFromAttack : IComponentData
{
    public WeaponType WeaponType;
    public AttackType AttackType;
}