using Damage;
using Health;
using Movement;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public partial struct EnemyAnimatorControllerSystem : ISystem
{
    private static string enemyTypeName = "enemyTypeID";
    private static string moveSpeedName = "movementSpeed";
    private static string attackTriggerName = "enemyAttack";
    private static string isAttackingName = "isAttacking";
    private static string hitStopTrigger = "hitStun";
    private static string knockBackTrigger = "hitKnockback";
    
    private static string deathName = "isKnockbacked";
    
    public void OnUpdate(ref SystemState state)
    {
        var deltaTime = SystemAPI.Time.DeltaTime;
        
        // setup enemy animators
        foreach (var (enemyAnimatorController, animatorReference, entity) in SystemAPI
            .Query<EnemyAnimatorControllerComponent, AnimatorReference>()
            .WithEntityAccess()
            .WithNone<HasSetupEnemyAnimator>())
        {
            animatorReference.Animator.SetInteger(enemyTypeName, (int)enemyAnimatorController.EnemyType);
            state.EntityManager.SetComponentEnabled<HasSetupEnemyAnimator>(entity, true);
        }
        
        // handle move animations
        foreach (var (animatorReference, moveSpeedComponent, entity) in SystemAPI
            .Query<AnimatorReference, MoveSpeedComponent>()
            .WithEntityAccess()
            .WithAll<HasSetupEnemyAnimator, EnemyAnimatorControllerComponent>())
        {
            float currentSpeed = animatorReference.Animator.GetFloat(moveSpeedName);
            float targetSpeed = moveSpeedComponent.IsMoving ? 1f : 0f;

            float lerpSpeed = 0.1f;

            float newSpeed = math.lerp(currentSpeed, targetSpeed, lerpSpeed);
            
            animatorReference.Animator.SetFloat(moveSpeedName, newSpeed);
        }
        
        // handle knock back - enable
        foreach (var (animatorReference, knockBackBuffer, entity) in SystemAPI
            .Query<AnimatorReference, DynamicBuffer<KnockBackBufferElement>>()
            .WithEntityAccess()
            .WithAll<HasSetupEnemyAnimator, EnemyAnimatorControllerComponent>()
            .WithNone<EnemyKnockBackAnimationComponent>())
        {
            if (knockBackBuffer.Length <= 0) continue;
            
            animatorReference.Animator.SetTrigger(knockBackTrigger);
            state.EntityManager.SetComponentEnabled<EnemyKnockBackAnimationComponent>(entity, true);
        }
        
        // handle knock back - disable
        foreach (var (animatorReference, knockBackBuffer, entity) in SystemAPI
            .Query<AnimatorReference, DynamicBuffer<KnockBackBufferElement>>()
            .WithEntityAccess()
            .WithAll<HasSetupEnemyAnimator, EnemyAnimatorControllerComponent, EnemyKnockBackAnimationComponent>())
        {
            state.EntityManager.SetComponentEnabled<EnemyKnockBackAnimationComponent>(entity, false);
        }
        
        // handle hit stuns - enable
        foreach (var (animatorReference, entity) in SystemAPI
            .Query<AnimatorReference>()
            .WithEntityAccess()
            .WithAll<HasSetupEnemyAnimator, EnemyAnimatorControllerComponent, HitStopComponent>()
            .WithNone<EnemyHitStunAnimationComponent>())
        {
            animatorReference.Animator.SetTrigger(hitStopTrigger);
            state.EntityManager.SetComponentEnabled<EnemyHitStunAnimationComponent>(entity, true);
        }
        
        // handle hit stuns - disable
        foreach (var (_, entity) in SystemAPI
            .Query<AnimatorReference>()
            .WithEntityAccess()
            .WithAll<HasSetupEnemyAnimator, EnemyAnimatorControllerComponent, EnemyHitStunAnimationComponent>()
            .WithNone<HitStopComponent>())
        {
            state.EntityManager.SetComponentEnabled<EnemyHitStunAnimationComponent>(entity, false);
        }

        // handle attack animations
        foreach (var (enemyAttackAnimation, animatorReference, entity) in SystemAPI
            .Query<RefRW<EnemyAttackAnimationComponent>, AnimatorReference>()
            .WithEntityAccess()
            .WithNone<HitStopComponent>()
            .WithAll<HasSetupEnemyAnimator, EnemyAnimatorControllerComponent>())
        {
            // Trigger attack animation
            if (!enemyAttackAnimation.ValueRO.HasSetTrigger)
            {
                animatorReference.Animator.SetTrigger(attackTriggerName);
                animatorReference.Animator.SetBool(isAttackingName, true);
                enemyAttackAnimation.ValueRW.HasSetTrigger = true;
            }
            
            // wait for attack animation to finish, then perform DOTS attack logic
            bool isAttacking = animatorReference.Animator.GetBool(isAttackingName);
            if (!isAttacking)
            {
                state.EntityManager.SetComponentEnabled<EnemyAttackAnimationComponent>(entity, false);
                state.EntityManager.SetComponentEnabled<ShouldAttackComponent>(entity, true);
                
                enemyAttackAnimation.ValueRW.HasSetTrigger = false;
            }

            // NOTE: TODO: Old code for using timers to trigger animations
            // // wait for attack animation to finish, then perform DOTS attack logic
            // enemyAttackAnimation.ValueRW.CurrentDelayTime += SystemAPI.Time.DeltaTime;
            // if (enemyAttackAnimation.ValueRO.CurrentDelayTime > enemyAttackAnimation.ValueRO.AnimationDelayTime)
            // {
            //     state.EntityManager.SetComponentEnabled<EnemyAttackAnimationComponent>(entity, false);
            //     state.EntityManager.SetComponentEnabled<ShouldAttackComponent>(entity, true);
            //     enemyAttackAnimation.ValueRW.CurrentDelayTime = 0;
            //     enemyAttackAnimation.ValueRW.HasSetTrigger = false;
            // }
        }
        
        // handle death animation
        foreach (var (animatorReference, hp, entity) in SystemAPI
            .Query< AnimatorReference, IsDyingComponent>()
            .WithEntityAccess()
            .WithAll<HasSetupEnemyAnimator, EnemyAnimatorControllerComponent>()
            .WithNone<EnemyDeathAnimationComponent>())
        {
            animatorReference.Animator.SetBool(deathName, true);
            state.EntityManager.SetComponentEnabled<EnemyDeathAnimationComponent>(entity, true);
        }
    }
}