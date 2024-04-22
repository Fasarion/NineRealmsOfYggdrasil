// using Unity.Burst;
// using Unity.Entities;
// using UnityEngine;
//
// public partial struct SpecialAttackChargeSystem : ISystem
// {
//     Entity currentChargeEntity;
//     
//     public void OnCreate(ref SystemState state)
//     {
//         state.RequireForUpdate<SpecialAttackChargeInfo>();
//     }
//     
//     [BurstCompile]
//     public void OnUpdate(ref SystemState state)
//     {
//         var specialAttackInfo = SystemAPI.GetSingletonRW<SpecialAttackChargeInfo>();
//
//         if (specialAttackInfo.ValueRO.IsCharging)
//         {
//             // spawn charge entity if it doesn't exist
//             if (currentChargeEntity == Entity.Null)
//             {
//                 foreach (var (weapon, chargeEntity) in SystemAPI.Query<WeaponComponent, SpecialAttackChargePrefab>().WithAll<ActiveWeapon>())
//                 {
//                     currentChargeEntity = state.EntityManager.Instantiate(chargeEntity.Value);
//                 }
//             }
//             
//             specialAttackInfo.ValueRW.ChargeTimer += SystemAPI.Time.DeltaTime;
//         }
//         else
//         {
//             specialAttackInfo.ValueRW.ChargeTimer = 0;
//             
//             // destroy charge entity if it exists
//             if (currentChargeEntity != Entity.Null)
//             {
//                 state.EntityManager.AddComponent<>()
//             }
//         }
//
//        
//     }
// }