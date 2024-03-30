using Unity.Entities;
using Unity.Transforms;

namespace Damage
{
    
    /// <summary>
    /// System Group for all systems related to handling damage.
    /// </summary>
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(FixedStepSimulationSystemGroup))]
    [UpdateAfter(typeof(TransformSystemGroup))]
    public partial class CombatSystemGroup : ComponentSystemGroup
    {
        
    }
}

