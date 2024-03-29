using Damage;
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
            
                AddComponent(entity, new MaxHpComponent
                {
                    Value = authoring.maxHP,
                });
                
                AddComponent(entity, new CurrentHpComponent
                {
                    Value = authoring.maxHP,
                });
                
                AddBuffer<DamageBufferElement>(entity);
            }
        }
    }

    public struct MaxHpComponent : IComponentData
    {
        public float Value;
    }
    
    public struct CurrentHpComponent : IComponentData
    {
        public float Value;
    }
}

