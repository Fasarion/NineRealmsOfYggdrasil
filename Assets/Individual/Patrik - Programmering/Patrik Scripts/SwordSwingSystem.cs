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
                    
                    if (_weaponManager == null)
                    {
                        Debug.LogError("Missing Player Weapon Handler.");
                        return;
                    }

                    _weaponManager.OnActiveWeaponStartAttackNormal += OnNormalAttackStart;
                    _weaponManager.OnActiveWeaponStopAttackNormal += OnNormalAttackStop;
                    
                    // disable sword collider at start
                    OnNormalAttackStop(new AttackData()); 
                    hasSetUp = true;
                }
            }
            
            // Handle normal attack
            PlayerFireInput fireInput = SystemAPI.GetSingleton<PlayerFireInput>();
            if (fireInput.FireKeyPressed)
            {
                _weaponManager.NormalAttack();
                return;
            }
            
            
            var specialAttack = SystemAPI.GetSingleton<PlayerSpecialAttackInput>();
            if (specialAttack.FireKeyPressed)
            {
                _weaponManager.SpecialAttack();
                return;
            }
        }

        

        protected override void OnStopRunning()
        {
            _weaponManager.OnActiveWeaponStartAttackNormal -= OnNormalAttackStart;
            _weaponManager.OnActiveWeaponStopAttackNormal -= OnNormalAttackStop;
        }

        void OnNormalAttackStart(AttackData data)
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
        
        private void OnNormalAttackStop(AttackData data)
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