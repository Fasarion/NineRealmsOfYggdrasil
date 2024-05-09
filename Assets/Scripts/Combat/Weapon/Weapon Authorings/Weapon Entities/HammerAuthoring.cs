using Unity.Entities;
using UnityEngine;

public class HammerAuthoring : MonoBehaviour
{
    class Baker : Baker<HammerAuthoring>
    {
        public override void Bake(HammerAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            
            AddComponent(entity, new HammerComponent());
        }
    }
}

public struct HammerComponent :  IComponentData{}