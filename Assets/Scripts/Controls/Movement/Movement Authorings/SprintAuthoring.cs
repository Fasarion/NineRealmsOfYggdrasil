using Unity.Entities;
using UnityEngine;

namespace Movement
{
    [RequireComponent(typeof(MoveSpeedAuthoring))]
    public class SprintAuthoring : MonoBehaviour
    {
        [Tooltip("How much the speed is modified by in a sprinting state.")]
        [SerializeField] private float sprintModifier = 1.5f;
    
        class Baker : Baker<SprintAuthoring>
        {
            public override void Bake(SprintAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);

                AddComponent(entity, new SprintComponent()
                {
                    SprintModifier = authoring.sprintModifier,
                });
            
            }
        }
    }
}