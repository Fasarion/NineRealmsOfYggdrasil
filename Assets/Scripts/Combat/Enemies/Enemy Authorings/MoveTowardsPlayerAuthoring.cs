using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class MoveTowardsPlayerAuthoring : MonoBehaviour
{
    [SerializeField] private float minimumDistanceForMoving;

    class Baker : Baker<MoveTowardsPlayerAuthoring>
    {
        public override void Bake(MoveTowardsPlayerAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new MoveTowardsPlayerComponent
            {
                MinimumDistanceForMoving = authoring.minimumDistanceForMoving
            });
        }
    }
}