using System.Collections;
using System.Collections.Generic;
using Movement;
using Patrik;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Serialization;

public enum AnimationEnemyType
{
    Melee = 0,
    Ranged = 1,
}

public class EnemyAnimatorControllerAuthoring : MonoBehaviour
{
    [Tooltip("Specifies enemy type in animator.")]
    public AnimationEnemyType EnemyAnimationType;
    
    [FormerlySerializedAs("attackDelay")]
    [Tooltip("How long the attack logic (like spawning a projectile) is delayed from the start of the attack animation.")]
    [SerializeField] private float attackDelayAfterAnimationStart = 0.6f;

    public bool waitForAttackToFinish = true;
    
    
    
    class Baker : Baker<EnemyAnimatorControllerAuthoring>
    {
        public override void Bake(EnemyAnimatorControllerAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new EnemyAnimatorControllerComponent
            {
                EnemyType = authoring.EnemyAnimationType
            }); 
            
            // Set up
            AddComponent(entity, new HasSetupEnemyAnimator { });
            SetComponentEnabled<HasSetupEnemyAnimator>(entity, false);
            
            // Attack
            AddComponent(entity, new EnemyAttackAnimationComponent
            {
                AnimationDelayTime = authoring.attackDelayAfterAnimationStart,
                WaitForAttackToFinish = authoring.waitForAttackToFinish
            });
            SetComponentEnabled<EnemyAttackAnimationComponent>(entity, false);
            
            // Hit Stun
            AddComponent(entity, new EnemyHitStunAnimationComponent { });
            SetComponentEnabled<EnemyHitStunAnimationComponent>(entity, false);
            
            // Knock Back
            AddComponent(entity, new EnemyKnockBackAnimationComponent { });
            SetComponentEnabled<EnemyKnockBackAnimationComponent>(entity, false);
            
            // Death
            AddComponent(entity, new EnemyDeathAnimationComponent() { });
            SetComponentEnabled<EnemyDeathAnimationComponent>(entity, false);
        }
    }
}

public struct EnemyAnimatorControllerComponent : IComponentData
{
    public AnimationEnemyType EnemyType;
}

public struct HasSetupEnemyAnimator : IComponentData, IEnableableComponent{}

public struct EnemyAttackAnimationComponent : IComponentData, IEnableableComponent
{
    public float AnimationDelayTime;
    public float CurrentDelayTime;
    public bool HasSetTrigger;
    public bool WaitForAttackToFinish;
}

public struct EnemyHitStunAnimationComponent : IComponentData, IEnableableComponent { }
public struct EnemyKnockBackAnimationComponent : IComponentData, IEnableableComponent { }
public struct EnemyDeathAnimationComponent : IComponentData, IEnableableComponent { }
