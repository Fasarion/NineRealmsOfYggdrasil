using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class BirdsNormalComboAttackConfigAuthoring : MonoBehaviour
{
   // [SerializeField] private int attackTornadoSpawnInterval = 4;

    class Baker : Baker<BirdsNormalComboAttackConfigAuthoring>
    {
        public override void Bake(BirdsNormalComboAttackConfigAuthoring configAuthoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);

            AddComponent(entity, new BirdComboAttackConfig
            {
               // attackTornadoSpawnInterval = configAuthoring.attackTornadoSpawnInterval,
                currentIndex = 0
            });
        }
    }
}

public struct BirdComboAttackConfig : IComponentData
{
   // public int attackTornadoSpawnInterval;
    public int currentIndex;
}