using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class MoveTowardsPlayerAuthoring : MonoBehaviour
{
    [Tooltip("The minimum distance from the player for this entity to move towards the player.")]
    [SerializeField] private float minimumDistanceForMoving;

    [Tooltip("The speed at which this entity moves away from the player if the player gets to close." +
             " A value of 0 means the entity will not move away at all.")]
    [SerializeField] private float awayFromPlayerMoveSpeed = 0.5f;

    class Baker : Baker<MoveTowardsPlayerAuthoring>
    {
        public override void Bake(MoveTowardsPlayerAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new MoveTowardsPlayerComponent
            {
                MinimumDistanceForMovingSquared = authoring.minimumDistanceForMoving * authoring.minimumDistanceForMoving,
                MoveAwayFromPlayerSpeed = authoring.awayFromPlayerMoveSpeed
            });
        }
    }
}