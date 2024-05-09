using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class HasOwnerWeaponAuthoring : MonoBehaviour
{
    class Baker:Baker<HasOwnerWeaponAuthoring>
    {
        public override void Bake(HasOwnerWeaponAuthoring weaponAuthoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new HasOwnerWeapon());
        }
    }
}

public struct HasOwnerWeapon : IComponentData
{
    public Entity OwnerEntity;
    public bool OwnerWasActive;
}
