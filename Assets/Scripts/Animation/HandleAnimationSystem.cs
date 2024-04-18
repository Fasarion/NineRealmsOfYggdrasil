using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

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
            var animator = animatorReference.Animator;
            if (!animator)
                continue;
            
            var animatorTransform = animator.transform;
            
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