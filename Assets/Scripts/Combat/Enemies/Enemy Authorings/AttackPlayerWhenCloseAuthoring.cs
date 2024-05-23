using Unity.Entities;
using UnityEngine;
using UnityEngine.Serialization;

public class AttackPlayerWhenCloseAuthoring : MonoBehaviour
{
    [FormerlySerializedAs("shootingCooldown")] [SerializeField] private float attackCooldown;
    [FormerlySerializedAs("shootingDelay")] [SerializeField] private float attackDelay;
    [FormerlySerializedAs("ditanceToPerformAttack")] [FormerlySerializedAs("shootDistance")] [SerializeField] private float distanceToPerformAttack;
    
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
            
            
            AddComponent(entity, new EnemyAttackAnimationComponent
            {
                AnimationDelayTime = authoring.attackDelay
            });
            SetComponentEnabled<EnemyAttackAnimationComponent>(entity, false);

            AddComponent(entity, new ShouldAttackComponent());
            SetComponentEnabled<ShouldAttackComponent>(entity, false);
        }
    }
}

public struct EnemyAttackAnimationComponent : IComponentData, IEnableableComponent
{
    public float AnimationDelayTime;
    public float CurrentDelayTime;
    public bool HasSetTrigger;
}

public struct ShouldAttackComponent : IComponentData, IEnableableComponent { }