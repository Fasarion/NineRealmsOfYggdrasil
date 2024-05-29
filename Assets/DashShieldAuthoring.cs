using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class DashShieldAuthoring : MonoBehaviour
{
    [SerializeField] private float offsetFromPlayer = 1f;
    
    class Baker : Baker<DashShieldAuthoring>
    {
        public override void Bake(DashShieldAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new DashShieldComponent
            {
                OffsetFromPlayer = authoring.offsetFromPlayer
            });
        }
    }
}

public struct DashShieldComponent : IComponentData
{
    public float OffsetFromPlayer;
}
