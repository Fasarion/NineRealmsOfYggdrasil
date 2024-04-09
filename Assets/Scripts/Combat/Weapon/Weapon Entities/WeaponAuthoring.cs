using Health;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

public class WeaponAuthoring : MonoBehaviour
{
    class Baker : Baker<WeaponAuthoring>
    {
        public override void Bake(WeaponAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            
            AddComponent(entity, new WeaponComponent());
            
            AddComponent(entity, new ActiveWeapon());
            SetComponentEnabled<ActiveWeapon>(entity, false);
        } 
    }
}

public struct WeaponComponent : IComponentData
{
    public LocalTransform AttackPoint;
}

public struct ActiveWeapon : IComponentData, IEnableableComponent { }
