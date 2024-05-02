
using Unity.Entities;
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

    [Tooltip("If the entity is supposed to follow the child of the prefab, mark this as true.")]
    [SerializeField] private bool followsChild;
    
    class Baker : Baker<AnimationAuthoring>
    {
        public override void Bake(AnimationAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponentObject(entity, new GameObjectAnimatorPrefab
            {
                Value = authoring.gameObjectPrefab,
                FollowEntity = authoring.followsEntity,
                FollowChild = authoring.followsChild
            });
        }
    }
}

public class GameObjectAnimatorPrefab : IComponentData
{
    public GameObject Value;
    public bool FollowEntity;
    public bool FollowChild;
}

public class AnimatorReference : ICleanupComponentData
{
    public Animator Animator;
}