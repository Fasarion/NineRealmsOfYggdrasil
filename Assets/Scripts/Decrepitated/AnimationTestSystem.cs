using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

[UpdateBefore(typeof(EnemyMovementSystem))]
[UpdateAfter(typeof(SpawnSystem))]
public partial struct AnimationTestSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<SpawnConfig>();
    }

    public void OnUpdate(ref SystemState state)
    {
        var config = SystemAPI.GetSingletonRW<SpawnConfig>();
        
        if (config.ValueRO.hasAnimated) return;
        
        var ecb = new EntityCommandBuffer(state.WorldUpdateAllocator);

        foreach (var (playerGameObjectPrefab, entity) in
                 SystemAPI.Query<PlayerGameObjectPrefab>().WithNone<PlayerAnimatorReference>().WithEntityAccess())
        {
            var newCompanionGameObject = Object.Instantiate((playerGameObjectPrefab.Value));
            var newAnimatorReference = new PlayerAnimatorReference
            {
                Value = newCompanionGameObject.GetComponent<Animator>()
            };
            ecb.AddComponent(entity, newAnimatorReference);
        }

        foreach (var (transform, animatorReference) in
                 SystemAPI.Query<LocalTransform, PlayerAnimatorReference>())
        {
            //animatorReference.Value.Play("turn_90_L");
            animatorReference.Value.transform.position = transform.Position;
            animatorReference.Value.transform.rotation = transform.Rotation;
        }
        
        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }
}
