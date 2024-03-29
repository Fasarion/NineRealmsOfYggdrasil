using System.Collections;
using System.Collections.Generic;
using Player;
using Unity.Entities;
using UnityEngine;

namespace Health
{
    public class HpTriggerSystemConfigBehaviour : MonoBehaviour
    {
    
        [Tooltip("Determines if the HP trigger system should run.")]
        [SerializeField] private bool enableHpTriggerSystem = true;

        class Baker : Baker<HpTriggerSystemConfigBehaviour>
        {
            public override void Bake(HpTriggerSystemConfigBehaviour authoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);

                if (authoring.enableHpTriggerSystem)
                {
                    AddComponent(entity, new HpTriggerConfig{});
                }
            }
        }
    }
    
    public struct HpTriggerConfig : IComponentData
    {
    }

}


