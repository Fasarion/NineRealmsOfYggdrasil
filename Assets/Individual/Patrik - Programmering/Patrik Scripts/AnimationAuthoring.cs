using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

public class AnimationAuthoring : MonoBehaviour
{
    [Tooltip("Game Object Prefab. Must have Animator attached for correct usage.")]
    [SerializeField] private GameObject gameObjectPrefab;
    
    class Baker : Baker<AnimationAuthoring>
    {
        public override void Bake(AnimationAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponentObject(entity, new GameObjectAnimatorPrefab{Value = authoring.gameObjectPrefab});
        }
    }
}

public class GameObjectAnimatorPrefab : IComponentData
{
    public GameObject Value;
}

public class AnimatorReference : ICleanupComponentData
{
    public Animator Value;
}

public partial struct HandleAnimationSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        var ecb = new EntityCommandBuffer(state.WorldUpdateAllocator);

        // Add animator references
        foreach (var (gameObjectPrefab, entity) in SystemAPI.Query<GameObjectAnimatorPrefab>()
            .WithNone<AnimatorReference>()
            .WithEntityAccess())
        {
            var gameObjectInstance = Object.Instantiate(gameObjectPrefab.Value);
            var animatorReference = new AnimatorReference()
            {
                Value = gameObjectInstance.GetComponent<Animator>()
            };
            ecb.AddComponent(entity, animatorReference);
        }
        
        // sync animator transform with corresponding entity transform
        foreach (var (transform, animatorReference) in
            SystemAPI.Query<LocalTransform, AnimatorReference>())
        {
            animatorReference.Value.transform.position = transform.Position;
            animatorReference.Value.transform.rotation = transform.Rotation;
        }
        
        // remove game objects when animator reference has been destroyed
        foreach (var (animatorReference, entity) in
            SystemAPI.Query<AnimatorReference>()
                .WithNone<LocalTransform, GameObjectAnimatorPrefab>()
                .WithEntityAccess())
        {
            Object.Destroy(animatorReference.Value.gameObject);
            ecb.RemoveComponent<AnimatorReference>(entity);
        }
        
        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }
}
