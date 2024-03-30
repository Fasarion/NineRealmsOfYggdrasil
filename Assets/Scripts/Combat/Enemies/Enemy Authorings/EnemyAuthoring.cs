using Unity.Entities;
using UnityEngine;

namespace AI
{
    public class EnemyAuthoring : MonoBehaviour
    {
        class Baker : Baker<EnemyAuthoring>
        {
            public override void Bake(EnemyAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new EnemyTag());
            }
        }
    }
}

