using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class SwordAuthoring : MonoBehaviour
{
    class Baker : Baker<SwordAuthoring>
    {
        public override void Bake(SwordAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            
            AddComponent(entity, new SwordComponent());
        }
    }
}

public struct SwordComponent :  IComponentData{}

public struct HammerComponent :  IComponentData{}
