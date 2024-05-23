using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class BirdPassiveAttackConfigAuthoring : MonoBehaviour
{
    [Header("Bird Settings")]
    [Tooltip("How many birds will spawn at once?")]
    [SerializeField] private int birdCount = 2;
    [Tooltip("How high above the player that the birds will spawn.")]
    [SerializeField] private float spawnHeight = 3f;
    [Tooltip("Lifetime of birds after spawning.")]
    [SerializeField] private float lifeTime = 2f;

    [Header("Seek Settings")]
    [Tooltip("After how many seconds will the birds start seeking enemies?")]
    [SerializeField] private float seekDelay = 0.3f;
    
    
    class Baker : Baker<BirdPassiveAttackConfigAuthoring>
    {
        public override void Bake(BirdPassiveAttackConfigAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.None);
            AddComponent(entity, new BirdPassiveAttackConfig
            {
                BirdCount = authoring.birdCount,
                SpawnHeight = authoring.spawnHeight,
                SeekDelay = authoring.seekDelay,
                LifeTime = authoring.lifeTime
            });
        }
    }
}

public struct BirdPassiveAttackConfig : IComponentData
{
    public int BirdCount;
    public float SpawnHeight;
    public float SeekDelay;
    public float LifeTime;
}
