using Unity.Entities;
using UnityEngine;

namespace AI
{
    public class EnemyTypeComponentAuthoring : MonoBehaviour
    {
        public EnemyType enemyType;
        
        class Baker : Baker<EnemyTypeComponentAuthoring>
        {
            public override void Bake(EnemyTypeComponentAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new EnemyTypeComponent
                {
                    Value = authoring.enemyType
                });
            }
        }
    }
}

