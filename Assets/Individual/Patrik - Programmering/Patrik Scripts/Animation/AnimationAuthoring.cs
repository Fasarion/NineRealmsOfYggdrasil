using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

public class AnimationAuthoring : MonoBehaviour
{
    [Tooltip("A Game Object with an animator component that will spawn at Start. This entity will match the game object's position and" +
             " rotation as well as let that object play animations.")]
    [SerializeField] private GameObject gameObjectPrefab;

    [Tooltip("This will force the game object to match its position and rotation with this entity." +
             "Otherwise, the entity will follow the game object. When an object's position is determined by " +
             "logic on the game object side (like an animator), this should be marked as False.")]
    [SerializeField] private bool followsEntity; 
    
    class Baker : Baker<AnimationAuthoring>
    {
        public override void Bake(AnimationAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponentObject(entity, new GameObjectAnimatorPrefab
            {
                Value = authoring.gameObjectPrefab,
                FollowEntity = authoring.followsEntity
            });
        }
    }
}

public class GameObjectAnimatorPrefab : IComponentData
{
    public GameObject Value;
    public bool FollowEntity;
}

public class AnimatorReference : ICleanupComponentData
{
    public Animator Animator;
}

public partial struct HandleAnimationSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        var ecb = new EntityCommandBuffer(state.WorldUpdateAllocator);

        // remove game objects when animator reference has been destroyed
        foreach (var (animatorReference, entity) in
            SystemAPI.Query<AnimatorReference>()
                .WithNone<LocalTransform, GameObjectAnimatorPrefab>()
                .WithEntityAccess())
        {
            Object.Destroy(animatorReference.Animator.gameObject);
            ecb.RemoveComponent<AnimatorReference>(entity);
        }
        
        // Add animator references
        foreach (var (gameObjectPrefab, entity) in SystemAPI.Query<GameObjectAnimatorPrefab>()
            .WithNone<AnimatorReference>()
            .WithEntityAccess())
        {
            var gameObjectInstance = Object.Instantiate(gameObjectPrefab.Value);
            var animatorReference = new AnimatorReference()
            {
                Animator = gameObjectInstance.GetComponent<Animator>()
            };
            ecb.AddComponent(entity, animatorReference);
        }
        
        // sync animator transform with corresponding entity transform
        foreach (var (transform, animatorReference, animatorObject) in
            SystemAPI.Query<RefRW<LocalTransform>, AnimatorReference, GameObjectAnimatorPrefab>())
        {
            var animatorTransform = animatorReference.Animator.transform;
            
            if (animatorObject.FollowEntity)
            {
                animatorTransform.position = transform.ValueRO.Position;
                animatorTransform.rotation = transform.ValueRO.Rotation;
            }
            else
            {
                transform.ValueRW.Position = animatorTransform.position;
                transform.ValueRW.Rotation = animatorTransform.rotation;
            }
        }
        
        
        
        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }
}
