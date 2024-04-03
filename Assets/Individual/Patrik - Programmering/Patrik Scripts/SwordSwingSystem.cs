using Damage;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

namespace Patrik
{
    public partial class SwordSwingSystem : SystemBase
    {
        private PlayerWeaponManagerBehaviour _weaponManager;

        private bool hasSetUp;

        protected override void OnUpdate()
        {
            if (!hasSetUp)
            {
                if (_weaponManager == null)
                {
                    _weaponManager = PlayerWeaponManagerBehaviour.Instance;

                    _weaponManager.OnActiveWeaponAttack += OnAttackPerformed;
                    _weaponManager.OnActiveWeaponStopAttack += OnAttackStop;
                }
            
                if (_weaponManager == null)
                {
                    Debug.LogError("Missing Player Weapon Handler.");
                    return;
                }

                hasSetUp = true;
            }
            
            
            PlayerFireInput fireInput = SystemAPI.GetSingleton<PlayerFireInput>();
            if (!fireInput.FireKeyPressed) return;
            
            _weaponManager.TryPerformCurrentAttack();
        }

        

        protected override void OnStopRunning()
        {
            _weaponManager.OnActiveWeaponAttack -= OnAttackPerformed;
        }

        void OnAttackPerformed()
        {
            var ecb = new EntityCommandBuffer(Allocator.Temp);
            
            
            foreach (var (transform, sword, entity) in SystemAPI.Query<RefRW<LocalTransform>, SwordComponent>()
                .WithAll<Disabled>()
                .WithEntityAccess())
            {
                ecb.RemoveComponent(entity, typeof(Disabled));
            }
            
            ecb.Playback(EntityManager);
        }
        
        private void OnAttackStop()
        {
            var ecb = new EntityCommandBuffer(Allocator.Temp);
            
            foreach (var (transform, sword, damageBuffer, entity) in 
                SystemAPI.Query<RefRW<LocalTransform>, SwordComponent, DynamicBuffer<HitBufferElement>>().WithEntityAccess())
            {
                ecb.AddComponent(entity, typeof(Disabled));
                
                damageBuffer.Clear();
            } 
            
            ecb.Playback(EntityManager);
        }
    }
}