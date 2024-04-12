using Health;
using Patrik;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

public class WeaponAuthoring : MonoBehaviour
{
    [Tooltip("Which type of weapon this is (Sword, Hammer, Mead etc).")]
    [SerializeField] private WeaponType weaponType;
    
    class Baker : Baker<WeaponAuthoring>
    {
        public override void Bake(WeaponAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            
            AddComponent(entity, new WeaponComponent
            {
                WeaponType = authoring.weaponType
            });
            
            AddComponent(entity, new ActiveWeapon());
            SetComponentEnabled<ActiveWeapon>(entity, false);
        } 
    }
}

public struct WeaponComponent : IComponentData
{
    public LocalTransform AttackPoint;
    public WeaponType WeaponType;
}

public struct ActiveWeapon : IComponentData, IEnableableComponent { }