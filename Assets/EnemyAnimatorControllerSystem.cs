using Unity.Entities;
using UnityEngine;

public partial struct EnemyAnimatorControllerSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        // setup enemy animators
        foreach (var (enemyAnimatorController, animatorReference, entity) in SystemAPI
            .Query<EnemyAnimatorControllerComponent, AnimatorReference>()
            .WithEntityAccess()
            .WithNone<HasSetupEnemyAnimator>())
        {
            animatorReference.Animator.SetInteger("enemyTypeID", (int)enemyAnimatorController.EnemyType);
            state.EntityManager.SetComponentEnabled<HasSetupEnemyAnimator>(entity, true);
        }

        // handle attack animations
        foreach (var (enemyAttackAnimation, animatorReference, entity) in SystemAPI
            .Query<RefRW<EnemyAttackAnimationComponent>, AnimatorReference>()
            .WithEntityAccess()
            .WithAll<HasSetupEnemyAnimator, EnemyAnimatorControllerComponent>())
        {
            // Trigger attack animation
            if (!enemyAttackAnimation.ValueRO.HasSetTrigger)
            {
                animatorReference.Animator.SetTrigger("enemyAttack");
                enemyAttackAnimation.ValueRW.HasSetTrigger = true;
            }

            // wait for attack animation to finish, then perform DOTS attack logic
            enemyAttackAnimation.ValueRW.CurrentDelayTime += SystemAPI.Time.DeltaTime;
            if (enemyAttackAnimation.ValueRO.CurrentDelayTime > enemyAttackAnimation.ValueRO.AnimationDelayTime)
            {
                state.EntityManager.SetComponentEnabled<EnemyAttackAnimationComponent>(entity, false);
                state.EntityManager.SetComponentEnabled<ShouldAttackComponent>(entity, true);
                enemyAttackAnimation.ValueRW.CurrentDelayTime = 0;
                enemyAttackAnimation.ValueRW.HasSetTrigger = false;
            }
        }
    }
}