using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class AudioCallerAuthoring : MonoBehaviour
{
    public class AudioCallerAuthoringBaker : Baker<AudioCallerAuthoring>
    {
        public override void Bake(AudioCallerAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.None);
            AddBuffer<AudioBufferData>(entity);
        }
    }
}

public struct AudioBufferData : IBufferElementData
{
    public int AudioEnumValue;
}


