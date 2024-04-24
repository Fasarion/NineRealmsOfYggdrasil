using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class ThunderStrikeAbilityAuthoring : MonoBehaviour
{
    public int strikeCounter;
    public bool isInitialized;

    public class ThunderStrikeAbilityAuthoringBaker : Baker<ThunderStrikeAbilityAuthoring>
    {
        public override void Bake(ThunderStrikeAbilityAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity,
                new ThunderStrikeAbility
                    {
                        strikeCounter = authoring.strikeCounter, isInitialized = authoring.isInitialized
                    });
        }
    }
}

public struct ThunderStrikeAbility : IComponentData
{
    public int strikeCounter;
    public bool isInitialized;
}
