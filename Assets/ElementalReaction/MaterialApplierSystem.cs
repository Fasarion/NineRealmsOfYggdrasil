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
                //materialType = state.EntityManager.GetComponentData<BaseMaterialReferenceComponent>(entity).Type;
                var rendererMaterialsReference = rendererReference.Renderer.materials;
                var newMaterials = new Material[1];
                var material = rendererMaterialsReference[0];
                newMaterials[0] = material;
                rendererReference.Renderer.materials = newMaterials;
            }
            else
            {
                var material = materialDatabase.GetMaterialReference(materialType);
                var rendererMaterialsReference = rendererReference.Renderer.materials;
                var newMaterials = new Material[2];
                for (int i = 0; i < rendererMaterialsReference.Length; i++)
                {
                    newMaterials[i] = rendererMaterialsReference[i];
                }
                newMaterials[newMaterials.Length - 1] = material;
                rendererReference.Renderer.materials = newMaterials;
            }

            ecb.RemoveComponent<ShouldChangeMaterialComponent>(entity);
        }
        
        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }
}
