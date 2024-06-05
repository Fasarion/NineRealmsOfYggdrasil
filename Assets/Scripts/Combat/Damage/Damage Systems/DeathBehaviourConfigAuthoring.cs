using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class DeathBehaviourConfigAuthoring : MonoBehaviour
{
    public float knockBackForceDamping = 0.8f;
    public float linearForceDamping = 1;
    public float inverseMass = 0.001f;
    public float minVelocity = 0.9f;

    public class DeathBehaviourConfigAuthoringBaker : Baker<DeathBehaviourConfigAuthoring>
    {
        public override void Bake(DeathBehaviourConfigAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity,
                new DeathBehaviourConfig
                {
                    KnockBackForceDamping = authoring.knockBackForceDamping,
                    LinearForceDamping = authoring.linearForceDamping,
                    InverseMass = authoring.inverseMass,
                    MinVelocity = authoring.minVelocity
                });
        }
    }
}

public struct DeathBehaviourConfig : IComponentData
{
    public float KnockBackForceDamping;
    public float LinearForceDamping;
    public float InverseMass;
    public float MinVelocity;
}
