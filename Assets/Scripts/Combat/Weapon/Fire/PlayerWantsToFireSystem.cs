// using System.Collections;
// using System.Collections.Generic;
// using Player;
// using Unity.Entities;
// using UnityEngine;
//
// namespace Weapon
// {
//     
//     [UpdateAfter(typeof(UpdateWeaponCooldownSystem))]
//     public partial struct PlayerWantsToFireSystem : ISystem
//     {
//         public void OnCreate(ref SystemState state)
//         {
//             state.RequireForUpdate<FireSettingsData>();
//           //  state.RequireForUpdate<PlayerWeaponConfig>();
//             state.RequireForUpdate<PlayerNormalAttackInput>();
//         }
//         
//         public void OnUpdate(ref SystemState state)
//         {
//             var fireSettings = SystemAPI.GetSingletonRW<FireSettingsData>();
//
//             if (fireSettings.ValueRO.autoFire)
//             {
//                 HandleAutoFire(ref state);
//             }
//             else
//             {
//                 HandleManualFire(ref state);
//             }
//         }
//
//         private void HandleAutoFire(ref SystemState state)
//         {
//             FireWithWeapons(ref state);
//         }
//
//         private void HandleManualFire(ref SystemState state)
//         {
//             bool fireButtonPressed = SystemAPI.GetSingleton<PlayerNormalAttackInput>().KeyPressed;
//             if (!fireButtonPressed) return;
//
//             FireWithWeapons(ref state);
//         }
//
//         private void FireWithWeapons(ref SystemState state)
//         {
//             foreach (var weapon in SystemAPI.Query<RefRW<ProjectileSpawnerComponent>>().WithAll<BelongsToPlayerTag>())
//             {
//                 if (weapon.ValueRO.HasCooledDown)
//                 {
//                     Fire(weapon);
//                 }
//             }
//         }
//         
//         private void Fire(RefRW<ProjectileSpawnerComponent> weapon)
//         {
//             weapon.ValueRW.WantsToFire = true;
//             ResetCoolDown(weapon);
//         }
//
//         private void ResetCoolDown(RefRW<ProjectileSpawnerComponent> weapon)
//         {
//             weapon.ValueRW.CurrentCoolDownTime = 0;
//         }
//     }
// }
