using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.VFX;


public partial struct VfxTestSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        
    }

    public void OnUpdate(ref SystemState state)
    {
        var ecb = new EntityCommandBuffer(state.WorldUpdateAllocator);
        
        foreach (var (playerGameObjectPrefab, entity) in
                 SystemAPI.Query<GameObjectPrefab>().WithNone<VfxReference>().WithEntityAccess())
        {
            var newCompanionGameObject = Object.Instantiate((playerGameObjectPrefab.Value));
            var newAnimatorReference = new VfxReference
            {
                vfxGraph = newCompanionGameObject.GetComponent<VisualEffect>(),
                particleSystem = newCompanionGameObject.GetComponent<ParticleSystem>()
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
