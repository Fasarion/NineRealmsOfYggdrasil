using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

namespace Destruction
{
    public class DestroyAfterSecondsAuthoring : MonoBehaviour
    {
        [Tooltip("After how many seconds will this object be destroyed?")]
        [SerializeField] private float lifeTime = 2f;
    
        class Baker : Baker<DestroyAfterSecondsAuthoring>
        {
            public override void Bake(DestroyAfterSecondsAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);
            
                AddComponent(entity, new DestroyAfterSecondsComponent
                {
                    TimeToDestroy = authoring.lifeTime
                });
            }
        }
    }

    struct DestroyAfterSecondsComponent : IComponentData
    {
        public float TimeToDestroy;
        public float CurrentLifeTime;
    }

    struct ShouldBeDestroyed : IComponentData, IEnableableComponent { }

}

