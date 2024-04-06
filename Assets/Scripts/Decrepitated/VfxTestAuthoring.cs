using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using UnityEngine.VFX;

public class GameObjectVfxPrefab : IComponentData
{
    public GameObject Value;
}

public class VfxReference : ICleanupComponentData
{
    public VisualEffect vfxGraph;
    public ParticleSystem particleSystem;
}

public class VfxTestAuthoring : MonoBehaviour
{
    public GameObject GameObjectPrefab;

    public class GameObjectPrefabBaker : Baker<VfxTestAuthoring>
    {
        public override void Bake(VfxTestAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponentObject(entity, new GameObjectVfxPrefab{Value = authoring.GameObjectPrefab});
        }
    }
}
