using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;


namespace Player
{
    public class PlayerMovementSystemConfigBehaviour : MonoBehaviour
    {
        [Tooltip("Determines if the player movement system should run")]
        [SerializeField] private bool playerMovementSystem = true;
        
    
        class Baker : Baker<PlayerMovementSystemConfigBehaviour>
        {
            public override void Bake(PlayerMovementSystemConfigBehaviour authoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);

                if (authoring.playerMovementSystem)
                {
                    AddComponent(entity, new PlayerMovementConfig{});
                }
            }
        }
    }

    public struct PlayerMovementConfig : IComponentData
    {
    }

   
    public struct PlayerWeaponConfig : IComponentData
    {
    }
    
    

}



