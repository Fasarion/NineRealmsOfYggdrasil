using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class ElementalVulnerableEffectConfigAuthoring : MonoBehaviour
{
    [Header("Vulnerable")]
    [Tooltip(
        "The additional damage the target will take while the vulnerable effect is applied. 0.5 for example means the target will take 50% extra damage while 1 means it will take 100% extra damage.")]
    public float vulnerableAdditionalDamagePercentage;

    public float vulnerableDuration;
    public float maxVulnurableStacks;

    public class ElementalVulnerableEffectConfigAuthoringBaker : Baker<ElementalVulnerableEffectConfigAuthoring>
    {
        public override void Bake(ElementalVulnerableEffectConfigAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity,
                new ElementalVulnerableEffectConfig
                {
                    VulnerableAdditionalDamagePercentage = authoring.vulnerableAdditionalDamagePercentage,
                    VulnerableDuration = authoring.vulnerableDuration,
                    MaxVulnurableStacks = authoring.maxVulnurableStacks
                });
        }
    }
}

public struct ElementalVulnerableEffectConfig : IComponentData
{
    public float VulnerableAdditionalDamagePercentage;
    public float VulnerableDuration;
    public float MaxVulnurableStacks;
}
