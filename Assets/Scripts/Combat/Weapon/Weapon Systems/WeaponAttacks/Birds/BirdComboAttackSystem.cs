using Patrik;
using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using Weapon;

[BurstCompile]
public partial struct BirdComboAttackSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<BirdComboAttackConfig>();
        state.RequireForUpdate<WeaponAttackCaller>();
        
        state.RequireForUpdate<GameUnpaused>();
    }
    
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var attackCaller = SystemAPI.GetSingletonRW<WeaponAttackCaller>();
        bool shouldStartAttack = attackCaller.ValueRO.ShouldStartActiveAttack(WeaponType.Birds, AttackType.Normal);
        if (!shouldStartAttack) return;
        
        var comboConfig = SystemAPI.GetSingletonRW<BirdComboAttackConfig>(); 
        var configEntity = SystemAPI.GetSingletonEntity<BirdComboAttackConfig>();
        
        if (!state.EntityManager.HasComponent<IsUnlocked>(configEntity)) return;

        var ecb = new EntityCommandBuffer(state.WorldUpdateAllocator);

        // Spawn projectiles (TODO: move to a general system, repeated code for this and bird special)
        foreach (var (transform, spawner, weapon, entity) in SystemAPI
            .Query<LocalTransform, ProjectileSpawnerComponent, WeaponComponent>()
            .WithAll<BirdsComponent>()
            .WithEntityAccess())
        {
            // update last index
            comboConfig.ValueRW.currentIndex++;

            if (comboConfig.ValueRW.currentIndex % comboConfig.ValueRO.attackTornadoSpawnInterval == 0)
            {
                state.EntityManager.SetComponentEnabled<ShouldSpawnBirdnado>(configEntity, true);
            }
        }
        
        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }
}