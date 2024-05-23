using Unity.Entities;
using Weapon;

public partial struct EnemyAttackSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        // attack with projectiles
        foreach (var (_, entity) in SystemAPI
            .Query<EnemyAnimatorControllerComponent>()
            .WithAll<ProjectileSpawnerComponent, ShouldAttackComponent, HasSetupEnemyAnimator>()
            .WithEntityAccess())
        {
            state.EntityManager.SetComponentEnabled<ShouldSpawnProjectile>(entity, true);
            state.EntityManager.SetComponentEnabled<ShouldAttackComponent>(entity, false);
        }
    }
}