using Unity.Entities;
using UnityEngine;

public class FrequencyAuthroing : MonoBehaviour
{
    [Tooltip("After how many attacks should this be activated?")]
    public int Frequency = 4;

    public class Baker : Baker<FrequencyAuthroing>
    {
        public override void Bake(FrequencyAuthroing authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new AbilityFrequencyComponent
            {
                
                Value = authoring.Frequency
            });
        }
    }
}

public struct AbilityFrequencyComponent : IComponentData
{
    public int Value;
}
