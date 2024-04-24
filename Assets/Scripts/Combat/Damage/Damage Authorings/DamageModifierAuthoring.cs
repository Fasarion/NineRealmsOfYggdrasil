using Patrik;
using Unity.Entities;
using UnityEngine;

public class DamageModifierAuthoring : MonoBehaviour
{
    [SerializeField] private float damageModifier = 1;

    class Baker : Baker<DamageModifierAuthoring>
    {
        public override void Bake(DamageModifierAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            
            AddComponent(entity, new DamageModifierComponent{Value = authoring.damageModifier});
            //AddComponent(entity, new CachedDamageModifierComponent{Value = authoring.damageModifier});
        }
    }
}

public struct DamageModifierComponent : IComponentData
{
    public float Value;
}

public struct SkillModifierComponent : IComponentData
{
    public AttackTypeModifier Value ;
}



[System.Serializable]
public struct AttackTypeModifier
{
    public float Normal;
    public float Special;
    public float Ultimate;
    public float Passive;

    public static readonly AttackTypeModifier Default = new AttackTypeModifier()
    {
        Normal = 1,
        Special = 1,
        Ultimate = 1,
        Passive = 1
    };

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