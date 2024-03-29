using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

namespace Health
{
    public class ChangeHPOnHitAuthroing : MonoBehaviour
    {
        [Tooltip("How much will the HP of this object change when hit? (a negative Value means HP is lost)")]
        [SerializeField] private float hpChangeValue = -1f;
    
        class Baker : Baker<ChangeHPOnHitAuthroing>
        {
            public override void Bake(ChangeHPOnHitAuthroing authoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);
            
                AddComponent(entity, new ChangeHPOnHit
                {
                    HPChangeValue = authoring.hpChangeValue
                });
            }
        }
    }

    public struct ChangeHPOnHit : IComponentData
    {
        public float HPChangeValue;
    }

}

