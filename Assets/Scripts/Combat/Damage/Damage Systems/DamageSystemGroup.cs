using Unity.Entities;
using Unity.Transforms;

namespace Damage
{
    
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(FixedStepSimulationSystemGroup))]
    [UpdateAfter(typeof(TransformSystemGroup))]
    public partial class DamageSystemGroup : ComponentSystemGroup
    {
        
    }
}

