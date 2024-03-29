using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Damage
{
    public struct HitBufferElement : IBufferElementData
    {
        public bool IsHandled;
        public float3 Position;
        public float3 Normal;
        public Entity HitEntity;
    }
}