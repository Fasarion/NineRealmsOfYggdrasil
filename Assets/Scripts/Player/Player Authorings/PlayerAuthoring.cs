using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

namespace Player
{
    public class PlayerAuthoring : MonoBehaviour
    {
        class Baker : Baker<PlayerAuthoring>
        {
            public override void Bake(PlayerAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent<PlayerTag>(entity);
                AddComponent<PlayerPositionSingleton>(entity);
                AddComponent<PlayerRotationSingleton>(entity);
                
                AddComponent<CanMoveFromInput>(entity);
                SetComponentEnabled<CanMoveFromInput>(entity, true);
                
                AddComponent<CanRotateFromInput>(entity);
                SetComponentEnabled<CanRotateFromInput>(entity, true);
            }
        }
    }
    
    public struct CanMoveFromInput : IComponentData, IEnableableComponent { }
    public struct CanRotateFromInput : IComponentData, IEnableableComponent { }
    
    public struct PlayerTag : IComponentData { }

}

