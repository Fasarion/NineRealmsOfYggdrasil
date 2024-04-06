using Unity.Entities;
using UnityEngine;

public class SwordStatsAuthoring : MonoBehaviour
{

    class Baker : Baker<SwordStatsAuthoring>
    {
        public override void Bake(SwordStatsAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.None);
            
            AddComponent(entity, new SwordStatsTag());
        }
    }
}