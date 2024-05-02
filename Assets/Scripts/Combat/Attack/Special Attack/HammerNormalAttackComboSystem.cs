using Player;
using Unity.Burst;
using Unity.Entities;
using UnityEngine;

namespace Patrik.Special_Attack
{
    public partial struct HammerNormalAttackComboSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<ThunderBoltConfig>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var abilityConfig = SystemAPI.GetSingleton<ThunderBoltConfig>();
            
            var attackCaller = SystemAPI.GetSingletonRW<WeaponAttackCaller>();

            if (!attackCaller.ValueRO.ShouldStartActiveAttack(WeaponType.Hammer, AttackType.Normal))
                return;

            int combo = attackCaller.ValueRO.ActiveAttackData.Combo;
            

            if (combo != 2) return;

            state.EntityManager.Instantiate(abilityConfig.AbilityPrefab);
        }
    }
}