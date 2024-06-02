using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class PlayerSpeedChangeConfigAuthoring : MonoBehaviour
{
    class Baker : Baker<PlayerSpeedChangeConfigAuthoring>
    {
        public override void Bake(PlayerSpeedChangeConfigAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.None);
            AddBuffer<PlayerMoveSpeedChangeElement>(entity);
        }
    }
}

public struct PlayerMoveSpeedChangeElement : IBufferElementData
{
    public float CurrentTime;
    public float MaxTime;
    public float SpeedChangeFactor;
}


