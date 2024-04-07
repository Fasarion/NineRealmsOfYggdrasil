using Unity.Entities;
using UnityEngine;

public class WeaponAuthoring : MonoBehaviour
{
    class Baker : Baker<WeaponAuthoring>
    {
        public override void Bake(WeaponAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            
            AddComponent(entity, new WeaponComponent());
        } 
    }
}

public struct WeaponComponent : IEnableableComponent, IComponentData
{
    public bool InActiveState;
}
