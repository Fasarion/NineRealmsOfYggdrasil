using Unity.Entities;
using UnityEngine;

namespace Movement
{
    public class MoveSpeedAuthoring : MonoBehaviour
    {
        [Tooltip("The speed in which this object moves.")]
        [SerializeField] private float speedValue;
    
        class Baker : Baker<MoveSpeedAuthoring>
        {
            public override void Bake(MoveSpeedAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);

                AddComponent(entity, new MoveSpeedComponent()
                {
                    Value = authoring.speedValue
                });
            }
        }
    }
}

