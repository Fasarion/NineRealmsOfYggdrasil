using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class VfxCallerAuthoring : MonoBehaviour
{
    public class VfxCallerAuthoringBaker : Baker<VfxCallerAuthoring>
    {
        public override void Bake(VfxCallerAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.None);
            AddBuffer<VfxBufferData>(entity);
        }
    }
}

public struct VfxBufferData : IBufferElementData
{
    public int VfxEnumValue;
}

