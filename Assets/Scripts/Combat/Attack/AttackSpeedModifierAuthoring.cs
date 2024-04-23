using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class AttackSpeedModifierAuthoring : MonoBehaviour
{
    public float value = 1;

    public class AttackSpeedModifierAuthoringBaker : Baker<AttackSpeedModifierAuthoring>
    {
        public override void Bake(AttackSpeedModifierAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new AttackSpeedModifier { Value = authoring.value });
        }
    }
}

public struct AttackSpeedModifier : IComponentData
{
    public float Value;
}
