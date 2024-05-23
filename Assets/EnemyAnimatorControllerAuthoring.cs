using System.Collections;
using System.Collections.Generic;
using Movement;
using Patrik;
using Unity.Entities;
using UnityEngine;

public enum AnimationEnemyType
{
    Melee = 0,
    Ranged = 1,
}

public class EnemyAnimatorControllerAuthoring : MonoBehaviour
{
    public AnimationEnemyType EnemyAnimationType;
    
    [Tooltip("How long the attack logic (like spawning a projectile) is delayed from the start of the attack animation.")]
    [SerializeField] private float attackDelay = 0.6f;
    
    class Baker : Baker<EnemyAnimatorControllerAuthoring>
    {
        public override void Bake(EnemyAnimatorControllerAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new EnemyAnimatorControllerComponent
            {
                EnemyType = authoring.EnemyAnimationType
            }); 
            
            AddComponent(entity, new HasSetupEnemyAnimator { });
            SetComponentEnabled<HasSetupEnemyAnimator>(entity, false);
            
            AddComponent(entity, new EnemyAttackAnimationComponent
            {
                AnimationDelayTime = authoring.attackDelay
            });
            SetComponentEnabled<EnemyAttackAnimationComponent>(entity, false);
            
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
}

