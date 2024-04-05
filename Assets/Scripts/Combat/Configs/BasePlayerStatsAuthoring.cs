using Unity.Entities;
using UnityEngine;

public class BasePlayerStatsAuthoring : MonoBehaviour
{

    class Baker : Baker<BasePlayerStatsAuthoring>
    {
        public override void Bake(BasePlayerStatsAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.None);
            
            AddComponent(entity, new BasePlayerStatsTag());
        }
    }
}