using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class MoveTowardsPlayerAuthoring : MonoBehaviour
{
    class Baker : Baker<MoveTowardsPlayerAuthoring>
    {
        public override void Bake(MoveTowardsPlayerAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new MoveTowardsPlayerComponent());
        }
    }
}