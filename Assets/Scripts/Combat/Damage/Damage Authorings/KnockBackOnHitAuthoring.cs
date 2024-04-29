using Unity.Entities;
using UnityEngine;

namespace Damage
{
    public class KnockBackOnHitAuthoring : MonoBehaviour
    {
        [Tooltip("Force to be applied on entity to knock it back on impact.")]
        [SerializeField] private float knockBackForce = 1;

        [Tooltip("In which direction will this entity knock back what it is hitting?,")]
        [SerializeField] private KnockDirectionType KnockDirection = KnockDirectionType.AlongHitNormal;
         
        public class Baker : Baker<KnockBackOnHitAuthoring>
        {
            public override void Bake(KnockBackOnHitAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
               
                AddComponent(entity, new KnockBackOnHitComponent
                {
                    Value = authoring.knockBackForce,
                    KnockDirection = authoring.KnockDirection
                });
            }
        }
    }
    
    public struct KnockBackOnHitComponent : IComponentData
    {
        public float Value;
        public KnockDirectionType KnockDirection;
    }

    public enum KnockDirectionType
    {
        AlongHitNormal,
        PerpendicularToPlayer,
        AwayFromPlayer,
    }
}