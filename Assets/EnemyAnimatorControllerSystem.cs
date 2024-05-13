using Unity.Entities;
using UnityEngine;
using Weapon;

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
            
            Debug.Log("Setup animator");
        }
        
        // fire animation
        foreach (var (enemyAnimatorController, animatorReference, entity) in SystemAPI
            .Query< EnemyAnimatorControllerComponent, AnimatorReference>()
            .WithEntityAccess()
            .WithAll<HasSetupEnemyAnimator, ShouldSpawnProjectile>())
        {
            animatorReference.Animator.SetTrigger("enemyAttack");
            Debug.Log("attack");
        }
    }
}