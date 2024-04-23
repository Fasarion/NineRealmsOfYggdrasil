using Unity.Entities;
using UnityEngine;

public class SkillModifierAuthoring : MonoBehaviour{
    
    [SerializeField] private AttackTypeModifier skillModifier = AttackTypeModifier.Default;

    class Baker : Baker<SkillModifierAuthoring>
    {
        public override void Bake(SkillModifierAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            
            AddComponent(entity, new SkillModifierComponent(){Value = authoring.skillModifier});
            //AddComponent(entity, new CachedSkillModifierComponent(){Value = authoring.skillModifier});
        }
    }
}