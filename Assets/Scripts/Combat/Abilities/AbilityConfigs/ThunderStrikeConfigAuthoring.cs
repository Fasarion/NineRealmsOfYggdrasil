using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class ThunderStrikeConfigAuthoring : MonoBehaviour
{
    public float test;
    public GameObject gluffs;

    public class ThunderStrikeConfigAuthoringBaker : Baker<ThunderStrikeConfigAuthoring>
    {
        public override void Bake(ThunderStrikeConfigAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity,
                new ThunderStrikeConfig
                    {
                        test = authoring.test, gluffs = GetEntity(authoring.gluffs, TransformUsageFlags.Dynamic)
                    });
        }
    }
}

public struct ThunderStrikeConfig : IComponentData
{
    public float test;
    public Entity gluffs;
}