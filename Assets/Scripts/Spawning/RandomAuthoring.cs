using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class RandomAuthoring : MonoBehaviour
{
    public class RandomAuthoringBaker : Baker<RandomAuthoring>
    {
        public override void Bake(RandomAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.None);
            AddComponent(entity, new RandomComponent
            {
                random = new Unity.Mathematics.Random(123)
            });
        }
    }
}




