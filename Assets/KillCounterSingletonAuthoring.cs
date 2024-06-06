using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class KillCounterSingletonAuthoring : MonoBehaviour
{
    public int value;

    public class KillCounterSingletonBaker : Baker<KillCounterSingletonAuthoring>
    {
        public override void Bake(KillCounterSingletonAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new KillCounterSingleton { Value = authoring.value });
        }
    }
}

public struct KillCounterSingleton : IComponentData
{
    public int Value;
}
