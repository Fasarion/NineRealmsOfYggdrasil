﻿using Unity.Entities;
using UnityEngine;

public class PlayerAttackHandlerAuthoring : MonoBehaviour
{
    class Baker : Baker<PlayerAttackHandlerAuthoring>
    {
        public override void Bake(PlayerAttackHandlerAuthoring spawnerHandlerAuthoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);

            AddComponent(entity, new PerformUltimateAttack());
            //AddComponent(entity, new SpecialAttackChargeInfo());
        }
    }
}

public partial struct PerformUltimateAttack : IComponentData
{
    public bool Value;
}


