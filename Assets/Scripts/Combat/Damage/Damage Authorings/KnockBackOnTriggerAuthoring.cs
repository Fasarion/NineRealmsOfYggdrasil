using Unity.Entities;
using UnityEngine;

namespace Damage
{
    public class KnockBackOnTriggerAuthoring : MonoBehaviour
    {
        [Tooltip("Force to be applied on entity to knock it back on impact.")]
        [SerializeField] private float knockBackForce = 1;
         
        public class CapabilityBaker : Baker<KnockBackOnTriggerAuthoring>
        {
            public override void Bake(KnockBackOnTriggerAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
               
                AddComponent(entity, new KnockBackForce
                {
                    Value = authoring.knockBackForce
                });
            }
        }
    }
}