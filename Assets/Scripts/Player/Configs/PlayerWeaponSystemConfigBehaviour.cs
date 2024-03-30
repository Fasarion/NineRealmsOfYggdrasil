using Unity.Entities;
using UnityEngine;

namespace Player
{
    public class PlayerWeaponSystemConfigBehaviour : MonoBehaviour
    {
        [Tooltip("Determines if the player aim system should run")]
        [SerializeField] private bool playerRotationSystem = true;
    
        [Tooltip("Determines if the player fire system should run")]
        [SerializeField] private bool playerFireSystem = true;
    
        class Baker : Baker<PlayerWeaponSystemConfigBehaviour>
        {
            public override void Bake(PlayerWeaponSystemConfigBehaviour authoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);

                if (authoring.playerRotationSystem)
                {
                    AddComponent(entity, new PlayerRotationConfig{});
                }
                
                if (authoring.playerFireSystem)
                {
                    AddComponent(entity, new PlayerWeaponConfig{});
                }
            }
        }
    }
    
    public struct PlayerRotationConfig : IComponentData
    {
        // non used "work around variable" to make the component existable as a singleton. Used as a RequireForUpdate-component
        // in player aim system.
        bool workAround;
    }

}