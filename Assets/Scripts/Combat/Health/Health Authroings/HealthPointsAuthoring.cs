using Damage;
using Destruction;
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
            
                // TODO: Create an aspect to group components and make functions
                AddComponent(entity, new MaxHpComponent {Value = authoring.maxHP,});
                AddComponent(entity, new CurrentHpComponent {Value = authoring.maxHP,});
                
                // Damage Buffer in order to be able to take damage
                AddBuffer<DamageBufferElement>(entity);
                
                // Tag to mark whenever an entity has changed HP in a given frame
                AddComponent<HasChangedHP>(entity);
                SetComponentEnabled<HasChangedHP>(entity, false);
                
                // Objects with health are assumed to be destroyed
                AddComponent(entity, new ShouldBeDestroyed());
                SetComponentEnabled<ShouldBeDestroyed>(false);
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

