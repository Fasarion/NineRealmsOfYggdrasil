using Unity.Entities;
using UnityEngine;

public class ChargeIndicatorAuthoring : MonoBehaviour
{
    class Baker : Baker<ChargeIndicatorAuthoring>
    {
        public override void Bake(ChargeIndicatorAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new ChargeIndicatorComponent());
        }
    }
}

public struct ChargeIndicatorComponent:IComponentData{}