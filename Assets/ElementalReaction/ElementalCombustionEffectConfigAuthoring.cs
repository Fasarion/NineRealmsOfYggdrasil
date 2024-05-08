using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class ElementalCombustionEffectConfigAuthoring : MonoBehaviour
{
    [Header("Combustion")] public float combustionDamage;
    public float combustionArea;

    public class ElementalCombustionEffectConfigAuthoringBaker : Baker<ElementalCombustionEffectConfigAuthoring>
    {
        public override void Bake(ElementalCombustionEffectConfigAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity,
                new ElementalCombustionEffectConfig
                {
                    CombustionDamage = authoring.combustionDamage, CombustionArea = authoring.combustionArea
                });
        }
    }
}

public struct ElementalCombustionEffectConfig : IComponentData
{
    public float CombustionDamage;
    public float CombustionArea;
}
