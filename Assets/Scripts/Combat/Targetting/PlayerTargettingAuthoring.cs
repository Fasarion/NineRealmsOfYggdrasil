using Unity.Entities;
using UnityEngine;

public class PlayerTargettingAuthoring : MonoBehaviour
{
    class Baker : Baker<PlayerTargettingAuthoring>
    {
        public override void Bake(PlayerTargettingAuthoring spawnerAuthoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            
            AddComponent(entity, new PlayerTargetingComponent());
        }
    }
}

public partial struct PlayerTargetingComponent : IComponentData{}