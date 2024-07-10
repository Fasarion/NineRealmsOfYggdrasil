using Damage;
using Destruction;
using Player;
using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

[BurstCompile]
[UpdateAfter(typeof(AttackStatTransferSystem))]
[UpdateAfter(typeof(PlayerRotationSystem))]
[UpdateAfter(typeof(CombatStatHandleSystem))]
public partial struct BirdnadoSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<MousePositionComponent>();
        state.RequireForUpdate<MousePositionInput>();
        state.RequireForUpdate<WeaponAttackCaller>();
        state.RequireForUpdate<PlayerTag>();
        state.RequireForUpdate<BirdnadoSpawnerComponent>();
        state.RequireForUpdate<GameUnpaused>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var playerEntity = SystemAPI.GetSingletonEntity<PlayerTag>();
        var playerTransform = state.EntityManager.GetComponentData<LocalTransform>(playerEntity);
        
        // spawn and initialize birdnado
        foreach (var (tornadoSpawner, entity) in SystemAPI.Query<BirdnadoSpawnerComponent>()
            .WithAll<ShouldSpawnBirdnado>()
            .WithEntityAccess())
        {
            // spawn tornado
            var tornado = state.EntityManager.Instantiate(tornadoSpawner.TornadoPrefab);
            
            // set size of tornado as configs diameter
            var tornadoTransform = state.EntityManager.GetComponentData<LocalTransform>(tornado);

            tornadoTransform.Position = playerTransform.Position;
            tornadoTransform.Rotation = playerTransform.Rotation;
            tornadoTransform.Scale = tornadoSpawner.TornadoRadius * 2;
            state.EntityManager.SetComponentData(tornado, tornadoTransform);
            
            // set tornado suction rate
            state.EntityManager.SetComponentData(tornado, new TimerObject{maxTime = tornadoSpawner.TimeBetweenSuctions});
            
            // play sound
            var audioBuffer = SystemAPI.GetSingletonBuffer<AudioBufferData>();
            audioBuffer.Add(new AudioBufferData { AudioData = tornadoSpawner.TornadoSound});
            
            // set damage
            CachedDamageComponent thisDamage = state.EntityManager.GetComponentData<CachedDamageComponent>(entity);
            thisDamage.Value.DamageValue *= tornadoSpawner.TornadoDamageMod;
            state.EntityManager.SetComponentData(tornado, thisDamage);
            
            state.EntityManager.SetComponentEnabled<ShouldSpawnBirdnado>(entity, false);
            
            state.EntityManager.SetComponentData(tornado, new DestroyAfterSecondsComponent {TimeToDestroy = tornadoSpawner.LifeTime});
        }
        
        // clear hitbuffer for suction
        foreach (var (transform, timer, hitBuffer) in SystemAPI
            .Query<RefRW<LocalTransform>, RefRW<TimerObject>, DynamicBuffer<HitBufferElement>>()
            .WithAll<BirdnadoComponent>())
        {
           // transform.ValueRW.Position = targetPos;
            timer.ValueRW.currentTime += SystemAPI.Time.DeltaTime;
                
            // clears hitbuffer based on timer
            if (timer.ValueRO.currentTime > timer.ValueRO.maxTime)
            {
                hitBuffer.Clear();
                timer.ValueRW.currentTime = 0;
            }
        }
    }
}