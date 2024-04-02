using Unity.Entities;
using UnityEngine;

namespace Health
{
    public class ChangeHPOnImpactAuthoring : MonoBehaviour
    {
        [Tooltip("How much HP will change for this object upon impact?")]
        [SerializeField] private float hpChange = -1f;
        
        class Baker : Baker<ChangeHPOnImpactAuthoring>
        {
            public override void Bake(ChangeHPOnImpactAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);
            
                AddComponent(entity, new ChangeHpOnImpact
                {
                    Value = authoring.hpChange
                });
            }
        }
    }
}