using Patrik;
using Unity.Entities;
using UnityEngine;

public class PlayerDamageModifiersAuthoring : MonoBehaviour
{
    [SerializeField] private float damageModifier;
    [SerializeField] private AttackTypeModifier skillModifier;

    class Baker : Baker<PlayerDamageModifiersAuthoring>
    {
        public override void Bake(PlayerDamageModifiersAuthoring modifiersAuthoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);

            AddComponent(entity, new PlayerDamageModifiersComponent
            {
                DamageModifier =modifiersAuthoring.damageModifier,
                AttackTypeModifier = modifiersAuthoring.skillModifier
            });
        }
    }
}

public struct PlayerDamageModifiersComponent : IComponentData
{
    public float DamageModifier;
    public AttackTypeModifier AttackTypeModifier;
}

[System.Serializable]
public struct AttackTypeModifier
{
    public float Normal;
    public float Special;
    public float Ultimate;
    public float Passive;

    public readonly float GetModifier(AttackType attackType)
    {
        switch (attackType)
        {
            case AttackType.Normal: return Normal;
            case AttackType.Special: return Special;
            case AttackType.Ultimate: return Ultimate;
            case AttackType.Passive: return Passive;
            
            default: return 1;
        }
    }
}