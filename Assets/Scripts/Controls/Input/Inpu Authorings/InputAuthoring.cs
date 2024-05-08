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
                
            // Move Inputs
            AddComponent<PlayerMoveInput>();
            AddComponent<PlayerDashInput>();
            
            // Mouse Inputs
            AddComponent<MousePositionInput>();

            // Attack Inputs
            AddComponent<PlayerNormalAttackInput>();
            AddComponent<PlayerSpecialAttackInput>();
            AddComponent<PlayerUltimateAttackInput>();
            
            // Weapon Switch Input
            AddComponent<WeaponOneInput>();
            AddComponent<WeaponTwoInput>();
            AddComponent<WeaponThreeInput>();

            // General Input
            AddComponent<PrimaryButtonInput>();
        }
    }
}
