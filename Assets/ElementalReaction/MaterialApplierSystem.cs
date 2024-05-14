using System.Collections;
using System.Collections.Generic;
using Damage;
using Unity.Entities;
using UnityEngine;

[UpdateInGroup(typeof(CombatSystemGroup))]
[UpdateBefore(typeof(HitStopSystem))]
public partial struct MaterialApplierSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        
    }

    public void OnUpdate(ref SystemState state)
    {
        var ecb = new EntityCommandBuffer(state.WorldUpdateAllocator);
        var materialDatabase = MaterialReferenceHolder.Instance;
        
        foreach (var (materialChange, rendererReference, entity) in SystemAPI
                     .Query<ShouldChangeMaterialComponent, SkinnedMeshRendererReference>()
                     .WithEntityAccess())
        {
            var materialType = materialChange.MaterialType;
            if (materialType == MaterialType.BASEMATERIAL)
            {
                materialType = state.EntityManager.GetComponentData<BaseMaterialReferenceComponent>(entity).Type;
            }
            var material = materialDatabase.GetMaterialReference(materialType);
            rendererReference.Renderer.material = material;
            ecb.RemoveComponent<ShouldChangeMaterialComponent>(entity);
        }
        
        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }
}
