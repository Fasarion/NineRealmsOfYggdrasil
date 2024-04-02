using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class TestEnemyAuthoring : MonoBehaviour
{
    public float2 velocity;

    public class EnemyAuthoringBaker : Baker<TestEnemyAuthoring>
    {
        public override void Bake(TestEnemyAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new TestEnemy { velocity = authoring.velocity });
        }
    }
}

public struct TestEnemy : IComponentData
{
    public float2 velocity;
}

