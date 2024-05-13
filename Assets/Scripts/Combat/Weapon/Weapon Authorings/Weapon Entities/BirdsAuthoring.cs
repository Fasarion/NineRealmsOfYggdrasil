using Unity.Entities;
using UnityEngine;

public class BirdsAuthoring : MonoBehaviour
{
    class Baker : Baker<BirdsAuthoring>
    {
        public override void Bake(BirdsAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            
            AddComponent(entity, new BirdsComponent());
        }
    }
}

public struct BirdsComponent :  IComponentData{}