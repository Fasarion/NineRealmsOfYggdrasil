using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class SwordComboAbilityConfigAuthoring : MonoBehaviour
{
    public int swordCount;
    public float radius;
    public float initialSpawnDelay;
    public float delayBetweenSwords;
    public GameObject swordProjectilePrefab;
    public GameObject swordComboAbilityPrefab;
    public GameObject swordSpawnEffectPrefab;
    public float offset;

    public class SwordComboAbilityConfigAuthoringBaker : Baker<SwordComboAbilityConfigAuthoring>
    {
        public override void Bake(SwordComboAbilityConfigAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity,
                new SwordComboAbilityConfig
                {
                    SwordCount = authoring.swordCount,
                    Radius = authoring.radius,
                    InitialSpawnDelay = authoring.initialSpawnDelay,
                    DelayBetweenSwords = authoring.delayBetweenSwords,
                    SwordProjectilePrefab = GetEntity(authoring.swordProjectilePrefab, TransformUsageFlags.Dynamic),
                    SwordComboAbilityPrefab =
                        GetEntity(authoring.swordComboAbilityPrefab, TransformUsageFlags.Dynamic),
                    SwordSpawnEffectPrefab =
                        GetEntity(authoring.swordSpawnEffectPrefab, TransformUsageFlags.Dynamic),
                    Offset = authoring.offset
                });
        }
    }
}

public struct SwordComboAbilityConfig : IComponentData
{
    public int SwordCount;
    public float Radius;
    public float InitialSpawnDelay;
    public float DelayBetweenSwords;
    public Entity SwordProjectilePrefab;
    public Entity SwordComboAbilityPrefab;
    public Entity SwordSpawnEffectPrefab;
    public float Offset;
    public bool HasRecorded;
}
    