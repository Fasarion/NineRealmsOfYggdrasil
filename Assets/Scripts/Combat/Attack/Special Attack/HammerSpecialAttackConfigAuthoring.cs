using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class HammerSpecialAttackConfigAuthoring : MonoBehaviour
{
    [SerializeField] private GameObject indicatorPrefab;
    
    class Baker : Baker<HammerSpecialAttackConfigAuthoring>
    {
        public override void Bake(HammerSpecialAttackConfigAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.None);
            AddComponent(entity, new HammerSpecialConfig{IndicatorPrefab = GetEntity(authoring.indicatorPrefab)});
        }
    }
}

public struct HammerSpecialConfig : IComponentData
{
    public Entity IndicatorPrefab;
}
