using System;
using Patrik;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Serialization;

public class HitSoundBasedOnAttackAuthoring : MonoBehaviour
{
    [SerializeField] private AudioData activeHitAudio;
    [SerializeField] private AudioData passiveHitAudio;
    
    class Baker : Baker<HitSoundBasedOnAttackAuthoring>
    {
        public override void Bake(HitSoundBasedOnAttackAuthoring authoring) 
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new HitSoundBasedOnAttackComponent
            {
                activeHitAudio = authoring.activeHitAudio,
                passiveHitAudio = authoring.passiveHitAudio
            });
        }
    }
}

public struct HitSoundBasedOnAttackComponent : IComponentData
{
    public AudioData activeHitAudio;
    public AudioData passiveHitAudio;
}

public partial struct SetWeaponBasedOnAttackTypeSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        foreach (var (weapon, hitSoundBasedOnAttackComponent, hitSound) in SystemAPI.Query<WeaponComponent, HitSoundBasedOnAttackComponent,
        RefRW<PlaySoundOnHittingComponent>>())
        {
            switch (weapon.CurrentAttackType)
            {
                case AttackType.Passive:
                    hitSound.ValueRW.Value = hitSoundBasedOnAttackComponent.passiveHitAudio;
                    break;
                
                default:
                    hitSound.ValueRW.Value = hitSoundBasedOnAttackComponent.activeHitAudio;
                    break;
            }
        }
    }
}