using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.VFX;

public class VfxAuthoring : MonoBehaviour
{
    //TODO: dubbelkolla slarvig copy paste
    
    [Tooltip("A Game Object with an animator component that will spawn at Start. This entity will match the game object's position and" +
             " rotation as well as let that object play animations.")]
    [SerializeField] private GameObject gameObjectPrefab;

    [Tooltip("This will force the game object to match its position and rotation with this entity." +
             "Otherwise, the entity will follow the game object. When an object's position is determined by " +
             "logic on the game object side (like an animator), this should be marked as False.")]
    [SerializeField] private bool followsEntity; 
    
    class Baker : Baker<VfxAuthoring>
    {
        public override void Bake(VfxAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponentObject(entity, new GameObjectVfxPrefab
            {
                Value = authoring.gameObjectPrefab,
                FollowEntity = authoring.followsEntity
            });
        }
    }
}

public class GameObjectVfxPrefab : IComponentData
{
    public GameObject Value;
    public bool FollowEntity;
}

public class VfxReference : ICleanupComponentData
{
    public VisualEffect Vfx;
}

public partial struct HandleVfxSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        var ecb = new EntityCommandBuffer(state.WorldUpdateAllocator);

        // remove game objects when animator reference has been destroyed
        foreach (var (animatorReference, entity) in
            SystemAPI.Query<VfxReference>()
                .WithNone<LocalTransform, GameObjectVfxPrefab>()
                .WithEntityAccess())
        {
            GameObject vfxObject = animatorReference.Vfx.gameObject;
            
            while (vfxObject.transform.parent != null)
            {
                vfxObject = vfxObject.transform.parent.gameObject;
            }
            
            Object.Destroy(vfxObject);
            ecb.RemoveComponent<VfxReference>(entity);
        }
        
        // Add animator references
        foreach (var (gameObjectPrefab, entity) in SystemAPI.Query<GameObjectVfxPrefab>()
            .WithNone<VfxReference>()
            .WithEntityAccess())
        {
            var gameObjectInstance = Object.Instantiate(gameObjectPrefab.Value);
            var animatorReference = new VfxReference()
            {
                Vfx = gameObjectInstance.GetComponentInChildren<VisualEffect>()
            };
            ecb.AddComponent(entity, animatorReference);
        }
        
        // sync animator transform with corresponding entity transform
        foreach (var (transform, vfxReference, vfxObject) in
            SystemAPI.Query<RefRW<LocalTransform>, VfxReference, GameObjectVfxPrefab>())
        {
            var vfx = vfxReference.Vfx;
            if (!vfx)
                continue;
            
            var vfxTransform = vfx.transform;
            
            if (vfxObject.FollowEntity)
            {
                vfxTransform.position = transform.ValueRO.Position;
                vfxTransform.rotation = transform.ValueRO.Rotation;
            }
            else
            {
                transform.ValueRW.Position = vfxTransform.position;
                transform.ValueRW.Rotation = vfxTransform.rotation;
            }
        }
        
        
        
        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }
}


