using Unity.Entities;
using UnityEngine;

public class BirdsAuthoring : MonoBehaviour
{
    class Baker : Baker<HammerAuthoring>
    {
        public override void Bake(HammerAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            
            AddComponent(entity, new BirdsComponent());
        }
    }
}

public struct BirdsComponent :  IComponentData{}