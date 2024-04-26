using Unity.Entities;
using UnityEngine;

public class HammerChargeAuthoring : MonoBehaviour
{
    class Baker : Baker<HammerChargeAuthoring>
    {
        public override void Bake(HammerChargeAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new HammerChargeComponent());
        }
    }
}

public struct HammerChargeComponent : IComponentData{}