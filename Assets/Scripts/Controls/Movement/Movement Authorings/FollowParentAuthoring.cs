using Unity.Entities;
using UnityEngine;

public class FollowParentAuthoring : MonoBehaviour
{
    [Tooltip("Parent that this entity will follow. The entity will match the parent's position and rotation exactly.")]
    [SerializeField] private GameObject parent;
    
    class Baker: Baker<FollowParentAuthoring>
    {
        public override void Bake(FollowParentAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            
            AddComponent(entity, new FollowParentComponent
            {
                ParentEntity = GetEntity(authoring.parent, TransformUsageFlags.Dynamic),
            });
        }
    }
}