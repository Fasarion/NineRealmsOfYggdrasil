using Patrik;
using Player;
using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

[UpdateBefore(typeof(CombatStatHandleSystem))]
public partial struct CombatManagedStatWriter : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<StatHandlerComponent>();
    }

    public void OnUpdate(ref SystemState state)
    {
        var statHandler = SystemAPI.GetSingletonRW<StatHandlerComponent>();
        if (!statHandler.ValueRO.ShouldUpdateStats)
            return;
        
        WriteOverAttackData(ref state);
    }
    
    void WriteOverAttackData(ref SystemState state)
    {
        var playerStatsEntity = SystemAPI.GetSingletonEntity<PlayerTag>();
        var playerSize = state.EntityManager.GetComponentData<SizeComponent>(playerStatsEntity).Value;
        
        // update size
        foreach (var (transform, cachedSize, animatorReference, sizeComponent, weapon, entity) in SystemAPI
            .Query<RefRW<LocalTransform>, RefRW<CachedSizeComponent>, AnimatorReference, SizeComponent, WeaponComponent>()
            .WithEntityAccess())
        {
            // TODO: change formula?
           float finalSize = 1 + playerSize + sizeComponent.Value;
           
            cachedSize.ValueRW.Value = finalSize;
            transform.ValueRW.Scale = finalSize;
            animatorReference.Animator.transform.localScale = Vector3.one * finalSize;
        }

        var playerAttackSpeed = state.EntityManager.GetComponentData<AttackSpeedModifier>(playerStatsEntity);

        var animator = state.EntityManager.GetComponentData<AnimatorReference>(playerStatsEntity);
        animator.Animator.SetFloat("attackSpeed", playerAttackSpeed.Value);
        
        // // Write Over Attack Speed
        // foreach (var (attackSpeedModifier, animatorReference, entity) in SystemAPI
        //     .Query<AttackSpeedModifier, AnimatorReference>()
        //     .WithEntityAccess())
        // {
        //     bool isPlayer = state.EntityManager.HasComponent<PlayerTag>(entity);
        //     if (isPlayer)
        //     {
        //         animatorReference.Animator.speed = attackSpeedModifier.Value;
        //     }
        //     else
        //     {
        //         animatorReference.Animator.speed = attackSpeedModifier.Value * playerAttackSpeed.Value;
        //     }
        // }
    }
}