using Unity.Entities;
using UnityEngine;
using Movement;

namespace Player
{
    
    public class PlayerMovementAuthoring : MonoBehaviour
    {
        [SerializeField] private float baseSpeed;
        [SerializeField] private float sprintModifier = 1.5f;
    
        class Baker : Baker<PlayerMovementAuthoring>
        {
            public override void Bake(PlayerMovementAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);

                AddComponent(entity, new SprintComponent()
                {
                    SprintModifier = authoring.sprintModifier,
                });
            
                AddComponent(entity, new MoveSpeedComponent()
                {
                    Value = authoring.baseSpeed,
                });
            }
        }
    }
    
    
}