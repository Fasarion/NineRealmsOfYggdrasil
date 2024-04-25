using Unity.Entities;
using UnityEngine;

public class SizeAuthoring : MonoBehaviour
{
    [SerializeField] private float value;
    
    public class Baker : Baker<SizeAuthoring>
    {
        public override void Bake(SizeAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new SizeComponent{Value = authoring.value});
            AddComponent(entity, new CachedSizeComponent{Value = authoring.value});
        }
    }
}

public struct SizeComponent : IComponentData
{
    public float Value;
}

public struct CachedSizeComponent : IComponentData
{
    public float Value;
}