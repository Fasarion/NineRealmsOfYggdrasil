using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

namespace Health
{
    public class HealthPointsAuthoring : MonoBehaviour
    {
        [Tooltip("Maximum health points of this object.")]
        [SerializeField] private float maxHP = 5;
    
        class Baker : Baker<HealthPointsAuthoring>
        {
            public override void Bake(HealthPointsAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);
            
                AddComponent(entity, new HealthPointsComponent
                {
                    Max = authoring.maxHP,
                    Current = authoring.maxHP
                });
            }
        }
    }

    struct HealthPointsComponent : IComponentData
    {
        public float Max;
        public float Current;
    }

}

