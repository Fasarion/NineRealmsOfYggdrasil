using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class InputAuthoring : MonoBehaviour
{
    class Baker : Baker<InputAuthoring>
    {
        public override void Bake(InputAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.None);
                
            AddComponent<PlayerMoveInput>();
            
            AddComponent<PlayerNormalAttackInput>();
            AddComponent<PlayerSpecialAttackInput>();
            AddComponent<PlayerUltimateAttackInput>();

            AddComponent<UpgradeUIButtonPress>();
        }
    }
}
