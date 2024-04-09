using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class InvincibleOnHitAuthoring : MonoBehaviour
{
    [SerializeField] private float invincibilityTime = 1f;

    class Baker : Baker<InvincibleOnHitAuthoring>
    {
        public override void Bake(InvincibleOnHitAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            
            AddComponent(entity, new InvincibilityComponent
            {
                InvincibilityTime = authoring.invincibilityTime
            });
            SetComponentEnabled<InvincibilityComponent>(entity, false);
        }
    }
}

public struct InvincibilityComponent : IEnableableComponent, IComponentData
{
    public float InvincibilityTime;
    public float CurrentTime;
}
