using Unity.Entities;
using UnityEngine;

namespace Player
{
    public class BelongsToPlayerAuthoring : MonoBehaviour
    {
        class Baker : Baker<BelongsToPlayerAuthoring>
        {
            public override void Bake(BelongsToPlayerAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent<BelongsToPlayerTag>(entity);
            }
        }
    }
}