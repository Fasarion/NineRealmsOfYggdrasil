using Health;
using Patrik;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

public class WeaponAuthoring : MonoBehaviour
{
    [Header("Weapon")]
    [Tooltip("Which type of weapon this is (Sword, Hammer, Mead etc).")]
    [SerializeField] private WeaponType weaponType;

    [Header("Attack")] 
    [Tooltip("Enable if this weapon's ultimate attack uses a target from the users mouse position.")]
    [SerializeField] private bool ultimateAttackUsesTarget;
    
    class Baker : Baker<WeaponAuthoring>
    {
        public override void Bake(WeaponAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            
            AddComponent(entity, new WeaponComponent
            {
                WeaponType = authoring.weaponType,
                UsesTargetingForUltimate = authoring.ultimateAttackUsesTarget
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
    
    public bool UsesTargetingForUltimate;
}

public struct ActiveWeapon : IComponentData, IEnableableComponent { }
