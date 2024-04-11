using Unity.Entities;
using UnityEngine;

public class EnergyFillAuthoring : MonoBehaviour
{
    [Tooltip("How much energy that will fill after a passive hit.")]
    [SerializeField] private float passiveFillPerHit = 5;
    
    [Tooltip("How much energy that the passive weapons will fill from an active attack with this entity.")]
    [SerializeField] private float activeFillPerHit = 5;

    class Baker : Baker<EnergyFillAuthoring>
    {
        public override void Bake(EnergyFillAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new EnergyFillComponent
            {
                PassiveFillPerHit = authoring.passiveFillPerHit,
                ActiveFillPerHit = authoring.activeFillPerHit
            });
        }
    }
}