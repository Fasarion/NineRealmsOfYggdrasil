using System.Collections;
using System.Collections.Generic;
using Damage;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using UnityEngine;

[UpdateInGroup(typeof(CombatSystemGroup))]
public partial struct ApplyElementalEffectSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var ecb = new EntityCommandBuffer(state.WorldUpdateAllocator);

        foreach (var (knockBackBuffer, physicsVelocity, damageReceivingEntity) in SystemAPI
                     .Query<DynamicBuffer<KnockBackBufferElement>, RefRW<PhysicsVelocity>>()
                     .WithEntityAccess())
        {
            float2 totalKnockBackForce = float2.zero;

            // Sum all knockbacks in buffer
            bool hasKnockback = false;
            foreach (var knockback in knockBackBuffer)
            {
                totalKnockBackForce += knockback.KnockBackForce;
                hasKnockback = true;
            }
            
            // Clear knock back buffer to avoid dealing damage multiple times an different frames
            knockBackBuffer.Clear();

            // Skip entity if no knock back should be dealt
            if (!hasKnockback)
                continue;

            // Knock back
            physicsVelocity.ValueRW.Linear += new float3(totalKnockBackForce.x, 0, totalKnockBackForce.y);
        }
        
        // Play back all operations in entity command buffer
        ecb.Playback(state.EntityManager);
    }
    
}
