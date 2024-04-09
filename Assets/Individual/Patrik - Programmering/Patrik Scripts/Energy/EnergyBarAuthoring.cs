using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class EnergyBarAuthoring : MonoBehaviour
{
    [Tooltip("How much energy it takes to fill the energy bar.")]
    [SerializeField] private float energyToFill = 100;

    class Baker : Baker<EnergyBarAuthoring>
    {
        public override void Bake(EnergyBarAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new EnergyBarComponent
            {
                MaxEnergy = authoring.energyToFill,
            });
        }
    }
}