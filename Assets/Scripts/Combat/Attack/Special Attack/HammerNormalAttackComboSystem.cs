using Unity.Entities;
using UnityEngine;

namespace Patrik.Special_Attack
{
    public partial struct HammerNormalAttackComboSystem : ISystem
    {
        public void OnUpdate(ref SystemState state)
        {
            var attackCaller = SystemAPI.GetSingletonRW<WeaponAttackCaller>();

            if (!attackCaller.ValueRO.ShouldStartActiveAttack(WeaponType.Hammer, AttackType.Normal))
                return;

            int combo = attackCaller.ValueRO.ActiveAttackData.Combo;
            // hej david skriv lite kod här
        }
    }
}