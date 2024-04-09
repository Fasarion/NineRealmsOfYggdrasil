using Unity.Entities;
using UnityEngine;

public class EnergyFillAuthoring : MonoBehaviour
{
    [Tooltip("How much energy that will fill after a hit.")]
    [SerializeField] private float fillPerHit = 5;

    class Baker : Baker<EnergyFillAuthoring>
    {
        public override void Bake(EnergyFillAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new EnergyFillComponent
            {
                FillPerHit = authoring.fillPerHit
            });
        }
    }
}