using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public struct RandomComponent : IComponentData
{
    public Unity.Mathematics.Random random;
    
}
