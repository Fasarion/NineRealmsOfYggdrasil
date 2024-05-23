using Unity.Entities;
using UnityEngine;
using UnityEngine.Serialization;

public class AttackPlayerWhenCloseAuthoring : MonoBehaviour
{
    [Tooltip("How long this entity has to wait between attacks.")]
    [SerializeField] private float attackCooldown;
    
    
    
    [Tooltip("How close to the player this entity needs to be to start attacking.")]
    [SerializeField] private float distanceToPerformAttack;
    
    class Baker : Baker<AttackPlayerWhenCloseAuthoring>
    {
        public override void Bake(AttackPlayerWhenCloseAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new AttackPlayerWhenCloseComponent
            {
                ShootingCooldownTime = authoring.attackCooldown,
                MinimumDistanceForShootingSquared = authoring.distanceToPerformAttack * authoring.distanceToPerformAttack
            });
            
            
           

            AddComponent(entity, new ShouldAttackComponent());
            SetComponentEnabled<ShouldAttackComponent>(entity, false);
        }
    }
}



public struct ShouldAttackComponent : IComponentData, IEnableableComponent { }