using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;

[UpdateInGroup(typeof(SimulationSystemGroup), OrderLast = true)]
[UpdateAfter(typeof(EndSimulationEntityCommandBufferSystem))]
public partial struct ResetGameVariablesSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
       state.RequireForUpdate<PlayerMoveInput>();
       state.RequireForUpdate<WeaponAttackCaller>();
    }
    
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        // Reset move input
        var moveInput = SystemAPI.GetSingletonRW<PlayerMoveInput>();
        moveInput.ValueRW.Value = float2.zero;

        var weaponCaller = SystemAPI.GetSingletonRW<WeaponAttackCaller>();

        weaponCaller.ValueRW.PassiveAttackData.ShouldStart = false;
        weaponCaller.ValueRW.PassiveAttackData.ShouldStop = false;
        weaponCaller.ValueRW.ActiveAttackData.ShouldStart = false;
        weaponCaller.ValueRW.ActiveAttackData.ShouldStop = false;
    }
}