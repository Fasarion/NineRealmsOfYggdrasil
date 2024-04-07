using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

namespace Player
{
    public class PlayerSettingsAuthoring : MonoBehaviour
    {
        [Header("Aiming")]
        [SerializeField] private bool autoAim;
        
        [Range(0,1)]
        [Tooltip("How fast will the player rotate? (0 means player always stands still, 1 means the players rotation is always " +
                 "synced with the mouse")]
        [SerializeField] private float rotationSpeed = 1;
    
        [Header("Firing")]
        [SerializeField] private bool autoFire;

        class Baker : Baker<PlayerSettingsAuthoring>
        {
            public override void Bake(PlayerSettingsAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);
                AddComponent(entity, new AimSettingsData()
                {
                    autoAim = authoring.autoAim,
                    rotationSpeed = authoring.rotationSpeed
                });
            
                AddComponent(entity, new FireSettingsData()
                {
                    autoFire = authoring.autoFire,
                });
            }
        }
    }

    public struct AimSettingsData : IComponentData
    {
        public bool autoAim;
        public float rotationSpeed;
    }

    public struct FireSettingsData : IComponentData
    {
        public bool autoFire;
    }


}


