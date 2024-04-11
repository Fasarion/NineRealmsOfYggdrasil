using Unity.Entities;
using UnityEngine;

public class HammerStatsAuthoring : MonoBehaviour
{

    class Baker : Baker<HammerStatsAuthoring>
    {
        public override void Bake(HammerStatsAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.None);
            
            AddComponent(entity, new HammerStatsTag());
        }
    }
}