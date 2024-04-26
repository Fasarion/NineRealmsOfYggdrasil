using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class ElementalReactionsConfigAuthoring : MonoBehaviour
{
    [Header("Burn")] public float burnDamage;
    public float burnDuration;
    public float maxBurnStacks;

    [Header("Shock")] public float shockDamage;
    public float shockDelay;
    public float maxShockStacks;

    [Header("Freeze")]
    [Tooltip(
        "How much freeze slows movement speed per stack on a scale from 0 to 1. 0.5 for example means that the movement is slowed by 50% while 1 means the movement is completely stopped.")]
    public float freezeMovementSlowPercentage;

    public float freezeDuration;
    public float maxFreezeStacks;

    [Header("Combustion")] public float combustionDamage;
    public float combustionArea;

    [Header("Vulnerable")]
    [Tooltip(
        "The additional damage the target will take while the vulnerable effect is applied. 0.5 for example means the target will take 50% extra damage while 1 means it will take 100% extra damage.")]
    public float vulnerableAdditionalDamagePercentage;

    public float vulnerableDuration;
    public float maxVulnurableStacks;

    [Header("Conduct")]
    [Tooltip(
        "The additional jumps that will be added to electrical effects like chain lightning. 1 means that the target will make the lightning jump to 1 additional target, 2 means 2 additional targets and so on.")]
    public int conductSpreadAddition;

    public float conductDuration;
    public float maxConductStacks;

    public class ElementalReactionsConfigAuthoringBaker : Baker<ElementalReactionsConfigAuthoring>
    {
        public override void Bake(ElementalReactionsConfigAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity,
                new ElementalReactionsConfig
                {
                    BurnDamage = authoring.burnDamage,
                    BurnDuration = authoring.burnDuration,
                    MaxBurnStacks = authoring.maxBurnStacks,
                    ShockDamage = authoring.shockDamage,
                    ShockDelay = authoring.shockDelay,
                    MaxShockStacks = authoring.maxShockStacks,
                    FreezeMovementSlowPercentage = authoring.freezeMovementSlowPercentage,
                    FreezeDuration = authoring.freezeDuration,
                    MaxFreezeStacks = authoring.maxFreezeStacks,
                    CombustionDamage = authoring.combustionDamage,
                    CombustionArea = authoring.combustionArea,
                    VulnerableAdditionalDamagePercentage = authoring.vulnerableAdditionalDamagePercentage,
                    VulnerableDuration = authoring.vulnerableDuration,
                    MaxVulnurableStacks = authoring.maxVulnurableStacks,
                    ConductSpreadAddition = authoring.conductSpreadAddition,
                    ConductDuration = authoring.conductDuration,
                    MaxConductStacks = authoring.maxConductStacks
                });
        }
    }
}

public struct ElementalReactionsConfig : IComponentData
{
    public float BurnDamage;
    public float BurnDuration;
    public float MaxBurnStacks;
    public float ShockDamage;
    public float ShockDelay;
    public float MaxShockStacks;
    public float FreezeMovementSlowPercentage;
    public float FreezeDuration;
    public float MaxFreezeStacks;
    public float CombustionDamage;
    public float CombustionArea;
    public float VulnerableAdditionalDamagePercentage;
    public float VulnerableDuration;
    public float MaxVulnurableStacks;
    public int ConductSpreadAddition;
    public float ConductDuration;
    public float MaxConductStacks;
}
