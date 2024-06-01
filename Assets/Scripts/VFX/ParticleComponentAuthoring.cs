using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.VFX;

public class ParticleComponentAuthoring : MonoBehaviour
{
    [Tooltip(
        "A Game Object with an animator component that will spawn at Start. This entity will match the game object's position and" +
        " rotation as well as let that object play animations.")]
    [SerializeField]
    private GameObject gameObjectPrefab;

    [Tooltip("This will force the game object to match its position and rotation with this entity." +
             "Otherwise, the entity will follow the game object. When an object's position is determined by " +
             "logic on the game object side (like an animator), this should be marked as False.")]
    [SerializeField]
    private bool followsEntity;
    

class Baker : Baker<ParticleComponentAuthoring>
    {
        public override void Bake(ParticleComponentAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponentObject(entity, new GameObjectParticlePrefab
            {
                Value = authoring.gameObjectPrefab,
                FollowEntity = authoring.followsEntity
            });
        }
    }
}

public class GameObjectParticlePrefab : IComponentData, IEnableableComponent
{
    public GameObject Value;
    public bool FollowEntity;
}

public class ParticleReference : ICleanupComponentData
{
    public ParticleSystem Particle;
}

[UpdateAfter(typeof(TransformSystemGroup))]
public partial struct HandleParticleSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        var ecb = new EntityCommandBuffer(state.WorldUpdateAllocator);

        // remove game objects when animator reference has been destroyed
        foreach (var (particleReference, entity) in
            SystemAPI.Query<ParticleReference>()
                .WithNone<LocalTransform, GameObjectParticlePrefab>()
                .WithEntityAccess())
        {
            Object.Destroy(particleReference.Particle.gameObject);
            ecb.RemoveComponent<ParticleReference>(entity);
        }
        
        // Add animator references
        foreach (var (gameObjectPrefab, entity) in SystemAPI.Query<GameObjectParticlePrefab>()
            .WithNone<ParticleReference>()
            .WithEntityAccess())
        {
            var gameObjectInstance = Object.Instantiate(gameObjectPrefab.Value, new Vector3(0, 1000f, 0), quaternion.identity);
            var particleReference = new ParticleReference()
            {
                Particle = gameObjectInstance.GetComponent<ParticleSystem>()
            };
            ecb.AddComponent(entity, particleReference);
        }
        
        // sync animator transform with corresponding entity transform
        foreach (var (transform, particleReference, particleObject) in
            SystemAPI.Query<RefRW<LocalTransform>, ParticleReference, GameObjectParticlePrefab>())
        {
            var particleSystem = particleReference.Particle;
            if (!particleSystem)
                continue;
            
            var particleSystemTransform = particleSystem.transform;
            
            if (particleObject.FollowEntity)
            {
                particleSystemTransform.position = transform.ValueRO.Position;
                particleSystemTransform.rotation = transform.ValueRO.Rotation;
                float scaleValue = transform.ValueRO.Scale;
                particleSystemTransform.localScale = new float3(scaleValue, scaleValue, scaleValue);
            }
            else
            {
                transform.ValueRW.Position = particleSystemTransform.position;
                transform.ValueRW.Rotation = particleSystemTransform.rotation;
                transform.ValueRW.Scale = particleSystemTransform.localScale.x;
            }
        }
        
        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }
}
