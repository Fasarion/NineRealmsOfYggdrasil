using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Rendering;

public class AreaAuthoring : MonoBehaviour
{
    [SerializeField] private float value;
    
    
    public class AreaAuthoringBaker : Baker<AreaAuthoring>
    {
        public override void Bake(AreaAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new AreaComponentData{Value = authoring.value});
        }
    }
}

public struct AreaComponentData : IComponentData
{
    public float Value;
}